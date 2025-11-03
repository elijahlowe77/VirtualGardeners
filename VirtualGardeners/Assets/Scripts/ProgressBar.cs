using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    public TMP_Text progressBarText; // TMP text of progress bar
    public int numPlants = 0; // number of plants in the garden
    public int numBloomed = 0; // number of plants that have fully bloomed

    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }

    // if we add a new plant to the garden
    public void addPlant() {
        numPlants++;
        Refresh();
    }

    // if we remove a plant in the garden
    public void removePlant() {
        numPlants = Mathf.Max(0, numPlants - 1);
        Refresh();
    }

    // if a plant has fully bloomed
    public void bloomPlant() {
        numBloomed++;
        Refresh();
    }

    // if we remove a fully bloomed plant, we call this AND removePlant()
    public void removeBloomPlant() {
        numBloomed = Mathf.Max(0, numBloomed - 1);
        removePlant();
        Refresh();
    }

    // refresh progress bar whenever theres an update
    private void Refresh() {
        // if theres no plants, set progress to 0%
        if(numPlants == 0) {
            progressBarText.text = "0% (0/0)";
            return;
        }

        // calculate progress
        float progress = (float)numBloomed / (float)numPlants;
        int progress_int = Mathf.RoundToInt(progress * 100f);
        progressBarText.text = progress_int + "%" + " " + "(" + numBloomed + " / " + numPlants + ")";
    }
}
