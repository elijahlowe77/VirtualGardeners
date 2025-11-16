using System.Collections;
using UnityEngine;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    [Header("Progress Displays")]
    public TMP_Text progressBarText; // TMP text of progress bar
    public UnityEngine.UI.Image circle; // circular progress indicator

    [Header("Tally Displays")] // ONLY the number part, not label
    public TMP_Text plantTally;
    public TMP_Text bushTally;
    public TMP_Text trashTally;
    public TMP_Text compostTally;

    [Header("Progress Weights")]
    public float plantWeight;       // Weight of planting
    public float bushWeight;        // Weight of bush pruning
    public float compostWeight;     // Weight of composting tasks
    public float trashWeight;       // Weight of garbage cleanup

    [Header("Collectible Containers")]
    public CollectibleContainer trashCan;
    public CollectibleContainer composter;

    private Dirt[] allDirt;
    private Bush[] allBushes;

    public float progressTest;

    void Start()
    {
        // Find all dirt objects
        allDirt = FindObjectsOfType<Dirt>();

        // Find all bush objects
        allBushes = FindObjectsOfType<Bush>();

        // Begin the slow loop
        StartCoroutine(SlowUpdate());
    }

    // Runs every second (like Update() but less frequent)
    IEnumerator SlowUpdate()
    {
        while (true)
        {
            // Update Progress
            UpdateProgress();
            UpdateTallies();

            // Sleep for 1s
            yield return new WaitForSeconds(1f); // wait 1 second
        }
    }

    // Returns percent value of planting tasks complete
    float GetDirtPercent()
    {
        float total = 0;
        foreach (Dirt dirt in allDirt)
        {
            // Value dirt state
            switch (dirt.state)
            {
                case "Hole":
                    total += 0.25f;
                    break;
                case "Seeded":
                    total += 0.50f;
                    break;
                case "Raked":
                    total += 0.75f;
                    break;
                default:
                    break;
            }

            // Value watering
            if (dirt.watered)
                total += 0.25f;
        }

        return total / allDirt.Length;
    }

    (int, int) GetDirtDone()
    {
        int done = 0;
        foreach (Dirt dirt in allDirt)
            if (dirt.state == "Raked" && dirt.watered)
                done++;

        return (done, allDirt.Length);
    }

    // Returns percent value of pruning tasks complete
    float GetBushesPercent()
    {
        float total = 0;
        foreach (Bush bush in allBushes)
            total += bush.state / 2; // state is 0, 1, or 2 -- translate to percent

        return total;
    }

    (int, int) GetBushesDone()
    {
        int done = 0;
        foreach (Bush bush in allBushes)
            if (bush.state == 2)
                done++;

        return (done, allBushes.Length);
    }

    // Refresh progress bar whenever theres an update
    private void UpdateProgress()
    {
        float progress = 0;

        // Calculate progress percentage
        progress += GetDirtPercent() * plantWeight;
        progress += GetBushesPercent() * bushWeight;
        progress += composter.GetPercentDone() * compostWeight;
        progress += trashCan.GetPercentDone() * trashWeight;

        progress = progressTest;

        // Display progress on watch
        int progress_int = Mathf.RoundToInt(progress * 100f);
        progressBarText.text = progress_int + "%"; // text
        circle.fillAmount = progress; // circle fill
    }

    private void UpdateTallies()
    {
        // Get progress tallies for (X / Y) task display
        var (dirtDone, dirtTotal) = GetDirtDone();
        var (bushesDone, bushesTotal) = GetBushesDone();
        var (trashDone, trashTotal) = composter.GetAmountDone();
        var (compostDone, compostTotal) = trashCan.GetAmountDone();

        // Write to sign
        plantTally.text = dirtDone + "/" + dirtTotal;
        bushTally.text = bushesDone + "/" + bushesTotal;
        trashTally.text = trashDone + "/" + trashTotal;
        compostTally.text = compostDone + "/" + compostTotal;
    }
}
