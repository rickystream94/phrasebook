package com.bobbytables.phrasebook.utils;

import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;

import androidx.appcompat.app.AppCompatActivity;

import android.util.Log;

import com.bobbytables.phrasebook.NewUserActivity;
import com.bobbytables.phrasebook.database.DatabaseHelper;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

/**
 * Created by ricky on 15/03/2017.
 */

public class SettingsManager {

    private static SettingsManager instance;
    private SharedPreferences preferences;
    private Editor editor;
    private Context context;
    // Shared preferences file name
    private static final String PREF_NAME = "MyPref";
    // All Shared Preferences Keys
    private static final String KEY_USER_EXISTS = "userExists";
    // User name
    public static final String KEY_NICKNAME = "nickname";
    public static final String KEY_CURRENT_LANG1 = "currentLang1Code";
    public static final String KEY_CURRENT_LANG2 = "currentLang2Code";
    public static final String KEY_CURRENT_LANG1_STRING = "currentLang1String";
    public static final String KEY_CURRENT_LANG2_STRING = "currentLang2String";
    public static final String KEY_TOTAL_XP = "totalXP";
    public static final String KEY_LEVEL = "level";
    public static final String KEY_CREATED = "created";
    public static final String KEY_PROFILE_PIC = "profilePic";

    private SettingsManager(Context context) {
        this.context = context;
        preferences = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        editor = preferences.edit();
        editor.apply();
        instance = this;
    }

    /**
     * Singleton method to retrieve settings manager
     *
     * @param context
     * @return
     */
    public static SettingsManager getInstance(Context context) {
        if (instance == null)
            return new SettingsManager(context);
        return instance;
    }

    public void createUserProfile(Context parentContext) {
        boolean userProfileExists = preferences.getBoolean(KEY_USER_EXISTS, false);
        if (userProfileExists)
            return;
        //User must be redirected to first activity
        Intent i = new Intent(context, NewUserActivity.class);
        // Add new Flag to start new Activity
        i.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        // Starting Login Activity
        context.startActivity(i);
        //Kill main activity
        ((AppCompatActivity) parentContext).finish();
        //MainActivity.killerHandler.sendEmptyMessage(0);
    }

    public void createUser(String nickname, int lang1Code, int lang2Code) {
        String currentTimeString = DateUtil.getCurrentTimestamp();
        editor.putString(KEY_NICKNAME, nickname);
        editor.putInt(KEY_CURRENT_LANG1, lang1Code);
        editor.putInt(KEY_CURRENT_LANG2, lang2Code);
        editor.putBoolean(KEY_USER_EXISTS, true);
        editor.putInt(KEY_TOTAL_XP, 0);
        editor.putInt(KEY_LEVEL, 0);
        editor.putString(KEY_CREATED, currentTimeString);
        editor.putString(KEY_PROFILE_PIC, "DEFAULT"); //is updated in version 2!
        editor.commit();
    }

    /**
     * Get a generic shared preference string value given a specific key
     *
     * @param key
     * @return
     */
    public String getPrefStringValue(String key) {
        return preferences.getString(key, "");
    }

    /**
     * Returns a ContentValues object with the string values of the current languages
     *
     * @return
     */
    public ContentValues getCurrentLanguagesNames() {
        DatabaseHelper db = DatabaseHelper.getInstance(context);
        String lang1 = db.getLanguageName(getPrefIntValue(KEY_CURRENT_LANG1));
        String lang2 = db.getLanguageName(getPrefIntValue(KEY_CURRENT_LANG2));
        ContentValues cv = new ContentValues();
        cv.put(KEY_CURRENT_LANG1_STRING, lang1);
        cv.put(KEY_CURRENT_LANG2_STRING, lang2);
        return cv;
    }

    /**
     * Returns a ContentValues object with the IDs of the current languages
     *
     * @return
     */
    public ContentValues getCurrentLanguagesIds() {
        ContentValues cv = new ContentValues();
        cv.put(KEY_CURRENT_LANG1, getPrefIntValue(KEY_CURRENT_LANG1));
        cv.put(KEY_CURRENT_LANG2, getPrefIntValue(KEY_CURRENT_LANG2));
        return cv;
    }

    /**
     * Get a generic shared preference integer value given a specific key
     *
     * @param key
     * @return
     */
    public int getPrefIntValue(String key) {
        return preferences.getInt(key, -1);
    }

    /**
     * Get a generic shared preference boolean value given a specific key
     *
     * @param key
     * @return
     */
    public boolean getPrefBoolValue(String key) {
        return preferences.getBoolean(key, false);
    }

    /**
     * It will be used to update the current level of the user
     *
     * @param key
     * @param value
     */
    public void updatePrefValue(String key, int value) {
        editor.putInt(key, value);
        editor.commit();
    }

    /**
     * Used to update profile pic
     *
     * @param key
     * @param value
     */
    public void updatePrefValue(String key, String value) {
        editor.putString(key, value);
        editor.commit();
    }

    public void updatePrefValue(String key, boolean value) {
        editor.putBoolean(key, value);
        editor.commit();
    }

    /**
     * It will be used to add experience points
     *
     * @param key
     * @param newValue
     */
    public void addValue(String key, int newValue) {
        int currentValue = preferences.getInt(key, 0);
        editor.putInt(key, currentValue + newValue);
        editor.commit();
    }

    /**
     * Returns all user's data in a JSONObject, used to export data
     *
     * @return
     */
    public JSONArray getUserData() throws JSONException {
        JSONArray resultSet = new JSONArray();
        JSONObject rowObject = new JSONObject();
        rowObject.put(KEY_NICKNAME, getPrefStringValue(KEY_NICKNAME));
        rowObject.put(KEY_CREATED, getPrefStringValue(KEY_CREATED));
        rowObject.put(KEY_LEVEL, getPrefIntValue(KEY_LEVEL));
        rowObject.put(KEY_TOTAL_XP, getPrefIntValue(KEY_TOTAL_XP));
        resultSet.put(rowObject);
        Log.d("USER DATA DUMP:", resultSet.toString());
        return resultSet;
    }

    public void resetXP() {
        editor.putInt(KEY_TOTAL_XP, 0);
        editor.putInt(KEY_LEVEL, 0);
        editor.commit();
    }
}
