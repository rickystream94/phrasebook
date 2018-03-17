package com.bobbytables.phrasebook;

import android.Manifest;
import android.app.Activity;
import android.app.ProgressDialog;
import android.content.ContentValues;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.design.widget.BottomNavigationView;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.util.Log;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.Toast;

import com.bobbytables.phrasebook.database.DatabaseHelper;
import com.bobbytables.phrasebook.database.PhrasebookModel;
import com.bobbytables.phrasebook.utils.AlertDialogManager;
import com.bobbytables.phrasebook.utils.FileManager;
import com.bobbytables.phrasebook.utils.SettingsManager;
import com.github.clans.fab.FloatingActionMenu;
import com.crashlytics.android.Crashlytics;

import io.fabric.sdk.android.Fabric;

import java.io.IOException;
import java.lang.ref.WeakReference;
import java.util.List;

public class MainActivity extends AppCompatActivity implements BottomNavigationView.OnNavigationItemSelectedListener {

    private SettingsManager settingsManager;
    private FloatingActionMenu fabMenu;
    private DatabaseHelper databaseHelper;
    private DrawerLayout mDrawerLayout;
    private Fragment fragment;
    private FragmentManager fragmentManager;
    private BottomNavigationView navigation;
    private List<PhrasebookModel> allPhrasebooks;
    private static final int WRITE_PERMISSION_REQUEST_CODE = 1;
    private static final int READ_PERMISSION_REQUEST_CODE = 2;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Fabric.with(this, new Crashlytics());
        setContentView(R.layout.activity_main);

        //Get helper classes
        databaseHelper = DatabaseHelper.getInstance(getApplicationContext());
        Crashlytics.setInt("Database Version", databaseHelper.getDatabaseVersion());
        settingsManager = SettingsManager.getInstance(getApplicationContext());
        Crashlytics.setString(SettingsManager.KEY_NICKNAME, settingsManager.getPrefStringValue
                (SettingsManager.KEY_NICKNAME));

        //Check always if it's the first time
        //Will invoke automatically NewUserActivity
        settingsManager.createUserProfile(this);

        //Get fragment manager and add default fragment
        fragmentManager = getSupportFragmentManager();
        FragmentTransaction transaction = fragmentManager.beginTransaction();
        transaction.setCustomAnimations(android.R.anim.fade_in, android.R.anim.fade_out);
        fragment = new CardsFragment();
        transaction.add(R.id.frame_layout, fragment).commit();

        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        getSupportActionBar().setTitle(R.string.tab1); //Set app bar title for default fragment

        //Initialize drawer
        mDrawerLayout = (DrawerLayout) findViewById(R.id.drawer_layout);
        ActionBarDrawerToggle mDrawerToggle = new ActionBarDrawerToggle(MainActivity.this, mDrawerLayout, toolbar, R
                .string.navigation_drawer_open, R.string.navigation_drawer_close);

        //Initialize bottom navigation
        navigation = (BottomNavigationView) findViewById(R.id.navigation);
        navigation.setOnNavigationItemSelectedListener(this);

        //Get list of phrasebooks in drawer list
        refreshPhrasebooks();

        //Initialize fab
        initFloatingActionButtons();
    }

    @Override
    protected void onResume() {
        super.onResume();
        refreshPhrasebooks();
    }

    /**
     * Method used to initialize the default navigation fragment when refreshing main activity
     * content
     */
    private void refreshUi() {
        navigation.setSelectedItemId(R.id.navigation_practice);
        mDrawerLayout.closeDrawers();
    }

    private void refreshPhrasebooks() {
        allPhrasebooks = databaseHelper.getAllPhrasebooks();
        String[] names = new String[allPhrasebooks.size()];
        for (PhrasebookModel phrasebook : allPhrasebooks)
            names[allPhrasebooks.indexOf(phrasebook)] = phrasebook.toString();
        ListView drawerList = (ListView) findViewById(R.id.left_drawer);
        drawerList.setAdapter(new ArrayAdapter<>(this, R.layout.drawer_list_item, R.id
                .drawer_list_item_id, names));
        drawerList.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> adapterView, View view, int position, long l) {
                PhrasebookModel phrasebook = allPhrasebooks.get(position);
                switchToPhrasebook(phrasebook);
            }
        });
    }

    private void switchToPhrasebook(PhrasebookModel phrasebook) {
        settingsManager.updatePrefValue(SettingsManager.KEY_CURRENT_LANG1, phrasebook.getLang1Code());
        settingsManager.updatePrefValue(SettingsManager.KEY_CURRENT_LANG2, phrasebook.getLang2Code
                ());
        refreshUi();
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        //Update Phrase Activity request code
        switch (requestCode) {
            case EditPhraseActivity.REQUEST_CODE:
                if (resultCode == RESULT_CANCELED) {
                    //It means that the database is empty and we need to refresh the layout
                    refreshUi();
                }
                break;
            case EditPhrasebookActivity.REQUEST_CODE:
                switch (resultCode) {
                    case RESULT_OK:
                        refreshUi();
                        break;
                    case RESULT_CANCELED:
                        //The current phrasebook has just been cancelled, therefore we need to
                        // switch to another phrasebook
                        PhrasebookModel firstPhrasebook = databaseHelper.getAllPhrasebooks().get(0);
                        switchToPhrasebook(firstPhrasebook);
                        Toast.makeText(this, "Switched to " + firstPhrasebook.toString() + " " +
                                "phrasebook", Toast.LENGTH_SHORT).show();
                        break;
                    default:
                        break;
                }
                break;
            case NewPhraseActivity.REQUEST_CODE:
                if (resultCode == RESULT_OK) {
                    refreshUi(); //It means the phrasebook is no more empty and we need to refresh
                }
                break;
            default:
                break;
        }
    }

    @Override
    //Remember: this method is invoked just once, exactly when the activity is created!
    //The return value states whether the menu will be active for the activity (true) or not (false)
    public boolean onCreateOptionsMenu(Menu menu) {
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.main_menu, menu);
        //Developer buttons, enable if needed
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        int id = item.getItemId();
        Intent i;
        final AlertDialog.Builder alertDialog;
        switch (id) {
            case R.id.delete_data:
                alertDialog = new AlertDialog.Builder(MainActivity.this);
                alertDialog.setTitle("Are you sure?");
                alertDialog.setMessage("All the data will be permanently deleted and cannot be " +
                        "recovered!");
                alertDialog.setPositiveButton(android.R.string.yes, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                        databaseHelper.reset();
                        Toast.makeText(MainActivity.this, "All data successfully deleted!", Toast
                                .LENGTH_SHORT)
                                .show();
                        refreshUi();
                    }
                });
                alertDialog.setNegativeButton(android.R.string.no, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                    }
                });
                alertDialog.show();
                break;
            case R.id.export_data:
                exportData();
                break;
            case R.id.import_data:
                alertDialog = new AlertDialog.Builder(MainActivity.this);
                alertDialog.setTitle("Are you sure?");
                alertDialog.setMessage("All the current data will be overwritten!");
                alertDialog.setPositiveButton(android.R.string.yes, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                        importDataFromBackup();
                    }
                });
                alertDialog.setNegativeButton(android.R.string.no, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                    }
                });
                alertDialog.show();
                break;
            case R.id.reset_xp:
                final AlertDialog.Builder resetAlertDialog = new AlertDialog.Builder(MainActivity
                        .this);
                resetAlertDialog.setTitle("Are you sure?");
                resetAlertDialog.setMessage("All the user data (XP points and level) will be " +
                        "permanently deleted and " +
                        "cannot be " +
                        "recovered!");
                resetAlertDialog.setPositiveButton(android.R.string.yes, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                        settingsManager.resetXP();
                        Toast.makeText(MainActivity.this, "Successfully reset user data!", Toast
                                .LENGTH_SHORT).show();
                    }
                });
                resetAlertDialog.setNegativeButton(android.R.string.no, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                    }
                });
                resetAlertDialog.show();
                break;
            case R.id.profile:
                i = new Intent(getApplicationContext(), ProfileActivity.class);
                startActivity(i);
                break;
            case R.id.edit_phrasebook:
                ContentValues cv = settingsManager.getCurrentLanguagesIds();
                Intent intent = new Intent(MainActivity.this, EditPhrasebookActivity.class);
                intent.putExtra(SettingsManager.KEY_CURRENT_LANG1, cv.getAsInteger(SettingsManager
                        .KEY_CURRENT_LANG1));
                intent.putExtra(SettingsManager.KEY_CURRENT_LANG2, cv.getAsInteger(SettingsManager.KEY_CURRENT_LANG2));
                startActivityForResult(intent, EditPhrasebookActivity.REQUEST_CODE);
                break;
            case R.id.about:
                i = new Intent(MainActivity.this, AboutActivity.class);
                startActivity(i);
                break;
            default:
                break;
        }
        return false;
    }

    /**
     * Setting floating action button with onClickListener
     */
    private void initFloatingActionButtons() {
        fabMenu = (FloatingActionMenu) findViewById(R.id.fab_menu);
        com.github.clans.fab.FloatingActionButton fabAddPhrase = (com.github.clans.fab.FloatingActionButton) findViewById(R.id
                .fab_new_phrase);
        com.github.clans.fab.FloatingActionButton fabCreatePhrasebook = (com.github.clans.fab.FloatingActionButton) findViewById(R.id
                .fab_new_phrasebook);
        fabAddPhrase.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent i = new Intent(getApplicationContext(), NewPhraseActivity.class);
                ContentValues currentLanguagesNames = settingsManager.getCurrentLanguagesNames();
                String lang1Value = currentLanguagesNames.getAsString(SettingsManager
                        .KEY_CURRENT_LANG1_STRING);
                String lang2Value = currentLanguagesNames.getAsString(SettingsManager
                        .KEY_CURRENT_LANG2_STRING);
                ContentValues currentLanguagesCodes = settingsManager.getCurrentLanguagesIds();
                int lang1Code = currentLanguagesCodes.getAsInteger(SettingsManager
                        .KEY_CURRENT_LANG1);
                int lang2Code = currentLanguagesCodes.getAsInteger(SettingsManager
                        .KEY_CURRENT_LANG2);
                i.putExtra(SettingsManager.KEY_CURRENT_LANG1_STRING, lang1Value);
                i.putExtra(SettingsManager.KEY_CURRENT_LANG2_STRING, lang2Value);
                i.putExtra(SettingsManager.KEY_CURRENT_LANG1, lang1Code);
                i.putExtra(SettingsManager.KEY_CURRENT_LANG2, lang2Code);
                startActivityForResult(i, NewPhraseActivity.REQUEST_CODE);
                fabMenu.close(true);
            }
        });
        fabCreatePhrasebook.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent i = new Intent(getApplicationContext(), NewPhrasebookActivity.class);
                startActivity(i);
                fabMenu.close(true);
            }
        });
        fabMenu.hideMenu(false); //by default
    }

    private boolean hasWritePermissions() {
        return ActivityCompat.checkSelfPermission(this,
                Manifest.permission.WRITE_EXTERNAL_STORAGE) == PackageManager.PERMISSION_GRANTED;
    }

    private boolean hasReadPermission() {
        return ActivityCompat.checkSelfPermission(this,
                Manifest.permission.READ_EXTERNAL_STORAGE)
                == PackageManager.PERMISSION_GRANTED;
    }

    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           @NonNull String permissions[], @NonNull int[] grantResults) {
        switch (requestCode) {
            case WRITE_PERMISSION_REQUEST_CODE: {
                // If request is cancelled, the result arrays are empty.
                if (grantResults.length > 0
                        && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    exportData();
                } else {
                    Toast.makeText(this, "Error, permission not granted!", Toast
                            .LENGTH_LONG).show();
                }
                break;
            }
            case READ_PERMISSION_REQUEST_CODE: {
                // If request is cancelled, the result arrays are empty.
                if (grantResults.length > 0
                        && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    importDataFromBackup();
                } else {
                    Toast.makeText(this, "Error, permission not granted!", Toast
                            .LENGTH_LONG).show();
                }
                break;
            }

        }
    }

    /**
     * Exports all data to JSON. First checks for permission.
     */
    private void exportData() {
        if (!hasWritePermissions()) {
            // We request the permission.
            Log.e("requesting", "write external permissions");
            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE}, WRITE_PERMISSION_REQUEST_CODE);
            return;
        }
        new ExportDataAsyncTask(this).execute();
    }

    private void importDataFromBackup() {
        if (!hasReadPermission()) {
            // We request the permission.
            Log.e("requesting", "read external permission");
            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.READ_EXTERNAL_STORAGE}, READ_PERMISSION_REQUEST_CODE);
            return;
        }
        new ImportDataAsyncTask(this).execute();
    }

    private void importDataSuccess() {
        List<PhrasebookModel> phrasebookModels = databaseHelper.getAllPhrasebooks();
        switchToPhrasebook(phrasebookModels.get(0));
        refreshPhrasebooks();
        Toast.makeText(MainActivity.this, "All data successfully restored from " +
                "latest backup!", Toast
                .LENGTH_LONG)
                .show();
    }

    /**
     * Checks if phone is connected to network
     *
     * @return true if connected, false otherwise
     */
    private boolean isConnected() {
        ConnectivityManager connMgr = (ConnectivityManager) getSystemService(Activity.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = connMgr.getActiveNetworkInfo();
        return networkInfo != null && networkInfo.isConnected();
    }

    @Override
    public boolean onNavigationItemSelected(@NonNull MenuItem item) {
        String title = item.getTitle().toString();
        getSupportActionBar().setTitle(title);
        switch (item.getItemId()) {
            case R.id.navigation_practice:
                fragment = new CardsFragment();
                fabMenu.hideMenu(true);
                break;
            case R.id.navigation_phrasebook:
                fragment = new PhrasesFragment();
                fabMenu.showMenu(true);
                break;
            case R.id.navigation_progress:
                fragment = new ProgressFragment();
                fabMenu.showMenu(true);
                break;
        }
        FragmentTransaction transaction = fragmentManager.beginTransaction();
        transaction.setCustomAnimations(android.R.anim.fade_in,
                android.R.anim.fade_out);
        transaction.replace(R.id.frame_layout, fragment).commitAllowingStateLoss();
        return true;
    }

    public void closeDrawer(View view) {
        mDrawerLayout.closeDrawers();
    }

    private static class ExportDataAsyncTask extends AsyncTask<Void, Void, Void> {

        private String occurredError;
        private String outputMessage;
        private ProgressDialog progressDialog;
        private WeakReference<MainActivity> activityWeakReference;
        private FileManager fileManager;

        ExportDataAsyncTask(MainActivity context) {
            activityWeakReference = new WeakReference<>(context);
        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            MainActivity activity = activityWeakReference.get();
            if (activity == null)
                return;
            fileManager = FileManager.getInstance(activity);
            progressDialog = new ProgressDialog(activity);
            progressDialog.setCancelable(false);
            progressDialog.setMessage("Exporting data, please wait...");
            progressDialog.show();
        }

        @Override
        protected Void doInBackground(Void... voids) {
            try {
                outputMessage = fileManager.exportDataToJSON();
            } catch (IOException e) {
                occurredError = e.getMessage();
            }
            return null;
        }

        @Override
        protected void onPostExecute(Void aVoid) {
            super.onPostExecute(aVoid);
            MainActivity activity = activityWeakReference.get();
            AlertDialogManager alertDialogManager = new AlertDialogManager();
            if (activity == null)
                return;
            if (progressDialog.isShowing())
                progressDialog.dismiss();
            if (outputMessage != null)
                Toast.makeText(activity, outputMessage, Toast.LENGTH_LONG).show();
            if (occurredError != null)
                alertDialogManager.showAlertDialog(activity, "Export error!", occurredError, false);
        }
    }

    private static class ImportDataAsyncTask extends AsyncTask<Void, Void, Void> {

        private String occurredError;
        private ProgressDialog progressDialog;
        private WeakReference<MainActivity> activityWeakReference;
        private FileManager fileManager;

        ImportDataAsyncTask(MainActivity context) {
            activityWeakReference = new WeakReference<>(context);
        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            MainActivity activity = activityWeakReference.get();
            if (activity == null)
                return;
            fileManager = FileManager.getInstance(activity);
            progressDialog = new ProgressDialog(activity);
            progressDialog.setCancelable(false);
            progressDialog.setMessage("Importing data from latest backup, please wait...");
            progressDialog.show();
        }

        @Override
        protected Void doInBackground(Void... voids) {
            try {
                fileManager.importDataFromBackup();
            } catch (Exception e) {
                occurredError = e.getMessage();
                e.printStackTrace();
            }
            return null;
        }

        @Override
        protected void onPostExecute(Void aVoid) {
            super.onPostExecute(aVoid);
            MainActivity activity = activityWeakReference.get();
            AlertDialogManager alertDialogManager = new AlertDialogManager();
            if (activity == null)
                return;
            if (progressDialog.isShowing())
                progressDialog.dismiss();
            if (occurredError != null)
                alertDialogManager.showAlertDialog(activity, "Import Error!", occurredError,
                        false);
            else
                activity.importDataSuccess();
        }
    }
}
