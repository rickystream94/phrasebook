package com.bobbytables.phrasebook;


import android.os.Bundle;

import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import jp.wasabeef.recyclerview.animators.SlideInRightAnimator;

public class CardsFragment extends Fragment {

    private RecyclerView cardsRecyclerView;
    private RecyclerView.Adapter cardsAdapter;
    private RecyclerView.LayoutManager recyclerViewLayoutManager;

    @Override
    public View onCreateView(LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {
        // The last two arguments ensure LayoutParams are inflated
        // properly.
        View rootView = inflater.inflate(R.layout.fragment_cards, container, false);
        //Handling cards recycler view
        cardsRecyclerView = (RecyclerView) rootView.findViewById(R.id.cardsRecyclerView);
        cardsRecyclerView.setHasFixedSize(true);
        cardsRecyclerView.setItemAnimator(new SlideInRightAnimator());
        recyclerViewLayoutManager = new LinearLayoutManager(getActivity());
        cardsRecyclerView.setLayoutManager(recyclerViewLayoutManager);
        cardsAdapter = new ChallengeCardsAdapter(getContext());
        cardsRecyclerView.setAdapter(cardsAdapter);
        return rootView;
    }
}