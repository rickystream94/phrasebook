package com.bobbytables.phrasebook.database;

import android.content.ContentValues;

/**
 * Created by ricky on 18/03/2017.
 */

public class ChallengeModel implements DatabaseModel {
    int phraseId;
    String createdOn;
    String tableName;
    int correct;
    int challengeId;


    public ChallengeModel(int phraseId, String createdOn, String tableName, int correct) {
        this.challengeId = UNSPECIFIED_ID_VALUE;
        this.phraseId = phraseId;
        this.createdOn = createdOn;
        this.tableName = tableName;
        this.correct = correct;
    }

    public ChallengeModel(int challengeId, int phraseId, String createdOn, String tableName, int
            correct) {
        this.challengeId = challengeId;
        this.phraseId = phraseId;
        this.createdOn = createdOn;
        this.tableName = tableName;
        this.correct = correct;
    }

    @Override
    public ContentValues getContentValues() {
        ContentValues contentValues = new ContentValues();
        if (this.challengeId != UNSPECIFIED_ID_VALUE)
            contentValues.put(DatabaseHelper.KEY_CHALLENGE_ID, this.challengeId);
        contentValues.put(DatabaseHelper.KEY_CHALLENGE_PHRASE_ID, this.phraseId);
        contentValues.put(DatabaseHelper.KEY_CREATED_ON, this.createdOn);
        contentValues.put(DatabaseHelper.KEY_CHALLENGE_CORRECT, this.correct);
        return contentValues;
    }

    @Override
    public String getTableName() {
        return this.tableName;
    }
}
