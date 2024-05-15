using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// The manager of a foundry.
// This class generates and keeps references to foundry intakes and ores, accepts player interaction, and checks answers

// IMPORTANT: The following code generate intakes and sign blocks exactly as the numbers returned from the windowQuestion
// Ex. if the question is 11 + 1 + 1 = 13, the forge will generate "__+_+_" and 4 ores of power 1
// Change this logic if necessary
public class FoundryManager : BaseInteractable
{
    [SerializeField] private GameObject intakePrefab;
    // The prefabs of the four types of signs
    // sign blocks connect groups of intakes and has math operation signs on it
    // A intake-sign group generated by this class may look like "___+__+____"
    [SerializeField] private GameObject additionSignPrefab;
    [SerializeField] private GameObject subtractionSignPrefab;
    [SerializeField] private GameObject multiplicationSignPrefab;

    [SerializeField] private GameObject orePrefab;
    // The temperary root transform of genereated ores. See Awake() below for its usage
    [SerializeField] private Transform oreTransformRoot;

    // The width of a intake/sign block. Used to determine the space between generating intakes and signs
    // It is assumed intakes and sign blocks share the same width. Add a new variable if this is changed
    [SerializeField] private float intakeWidth;
    // A reference to the WindowQuestion instance in the same level
    // This class generates itself based on the information from this instance
    [SerializeField] private WindowQuestion windowQuestion;
    // Unity event to dispatch when a weapon is forged
    [SerializeField] private UnityEvent OnWeaponForged;

    // Each element of this list is a list of intakes
    // IMPORTANT: This variable is designed to hold exactly as many intakes as the numbers returned from the windowQuestio
    // If the ith number is 4-digit, then intakeGroups[i] will be a list of 4 intakes
    // Change its definition if necessary
    private List<FoundryIntakeManager>[] intakeGroups;
    // The list of numbers retreived from windowQuestion that makes up the final answer
    private List<int> targetPowerLvs;
    private int targetPower;

    private TextboxBehavior tutorial;

    private void Awake()
    {
        
        if (UnityEngine.Random.Range(0f, 1f) < 0.3f) {
            windowQuestion.SetSubject(Subject.Multiplication);
        } else {
            windowQuestion.SetSubject(Subject.Addition);
        }

        // Prompt the windowQuestion to generate a question
        windowQuestion.GenerateInitialQuestion();
        List<int> enemyStrengths = windowQuestion.GetEnemyStrengths();

        tutorial = GameObject.Find("Tutorial Manager").GetComponent<TextboxBehavior>();

        StartCoroutine(SpawnDelay());
    }

    private IEnumerator SpawnDelay(){
        yield return new WaitForSeconds(0.1f);

        SpawnIntake();
    }

    private void SpawnIntake(){
        // Get solution from window question
        targetPower = windowQuestion.GetSolution();
        List<int> windowPowerLvs = windowQuestion.GetEnemyStrengths();
        // Generate intake question using the answer to the window question
        windowQuestion.GenerateIntakeQuestion(targetPower);
        // Retreive the numbers that makes up the final answer
        targetPowerLvs = windowQuestion.GetEnemyStrengths();
       
        //int targetPowerLvTotal;
        //switch (windowQuestion.subject)
        //{
        //    case Subject.Addition:
        //        targetPowerLvTotal = 0;
        //        foreach (int i in targetPowerLvs) targetPowerLvTotal += i;
        //        break;
        //    case Subject.Subtraction:
        //        targetPowerLvTotal = targetPowerLvs[0];
        //        for (int i = 1; i < targetPowerLvs.Count; i++) targetPowerLvTotal -= targetPowerLvs[i];
        //        break;
        //    case Subject.Multiplication:
        //        targetPowerLvTotal = 1;
        //        foreach (int i in targetPowerLvs) targetPowerLvTotal *= i;
        //        break;
        //}

        // The list of digits of each of the number in targetPowerLvs
        // That is, valDigits[i] is list holding the digits of targetPowerLvs[i]
        // If targetPowerLvs[i] = 1325, valDigits[i] = {1, 3, 2, 5}
        List<int>[] valDigits = new List<int>[targetPowerLvs.Count];
        // Total number of digits
        int totalDigits = 0;
        // Calculate valDigits
        for (int i = 0; i < targetPowerLvs.Count; i++)
        {
            valDigits[i] = new List<int>();
            int temp = targetPowerLvs[i];
            for (int j = 1; j < 10; j++)
            {
                if (temp <= 0) { break; }
                valDigits[i].Add(temp % 10);
                temp /= 10;
            }
            totalDigits += valDigits[i].Count;
        }
        // Add the number of sign blocks into totalDigits
        // If there are targetPowerLvs.Count numbers, there will be targetPowerLvs.Count - 1 sign blocks
        totalDigits += targetPowerLvs.Count - 1;

        // Initiate intakeGroups
        intakeGroups = new List<FoundryIntakeManager>[targetPowerLvs.Count];
        int digitCounter = 0;  // Number of all the intakes and sign blocks generated so far. Used to position blocks
        for (int i = 0; i < targetPowerLvs.Count; i++)
        {
            intakeGroups[i] = new List<FoundryIntakeManager>();
            // Generate an intake and an ore for each digit in valDigits
            for (int j = 0; j < valDigits[i].Count; j++)
            {
                // Generate an intake
                GameObject intake = Instantiate(intakePrefab, transform);
                intake.transform.localPosition = new Vector3(intakeWidth * (digitCounter - totalDigits / 2f + 0.5f), 0f, 0f);
                intakeGroups[i].Add(intake.GetComponent<FoundryIntakeManager>());

                // Allows intakes to tell the tutorial when an ore is inserted
                intake.GetComponent<FoundryIntakeManager>().SetTutorial(tutorial);

                // Generate an ore
                // The ore is originally generated with root/parent transform oreTransformRoot
                // This is so that all the ores are centered arround a point
                // The ore is later set to have parent transform of this.transform.parent (the room)
                GameObject ore = Instantiate(orePrefab, oreTransformRoot);
                Vector3 orePos = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * 3f;
                orePos.y = 3f;
                ore.transform.localPosition = orePos;
                ore.transform.parent = transform.parent;
                ore.GetComponent<OreManager>().SetPower(valDigits[i][j]);

                digitCounter++;
            }

            // Generate sign blocks between intake groups
            if (i + 1 == targetPowerLvs.Count) break;  // Signs are not generated after the last intake group
            GameObject signBlock;
            switch (windowQuestion.subject)
            {
                case Subject.Addition:
                    signBlock = Instantiate(additionSignPrefab, transform);
                    break;
                case Subject.Subtraction:
                    signBlock = Instantiate(subtractionSignPrefab, transform);
                    break;
                case Subject.Multiplication:
                    signBlock = Instantiate(multiplicationSignPrefab, transform);
                    break;
                default:
                    signBlock = Instantiate(additionSignPrefab, transform);
                    break;
            }
            signBlock.transform.localPosition = new Vector3(intakeWidth * (digitCounter - totalDigits / 2f + 0.5f), 0f, 0f);

            digitCounter++;
        }
    }

    // Check answer
    public override void OnInteract()
    {
        int totalAns = 0;
        switch (windowQuestion.subject)
        {
            case Subject.Addition:
                for (int i = 0; i < targetPowerLvs.Count; i++)
                {
                    int ans = 0;
                    foreach (FoundryIntakeManager intake in intakeGroups[i])
                    {
                        ans = ans * 10 + intake.GetPower();
                    }
                    //if this set of intakes is correct, give feedback
                    bool match = false;
                    foreach (int power in targetPowerLvs){
                        if (ans == power){
                            foreach (FoundryIntakeManager intake in intakeGroups[i]) {
                                intake.CorrectFeedback();
                            }
                            match = true;
                        }
                    } 
                    //if not, give incorrect feedback
                    if(!match){
                        foreach (FoundryIntakeManager intake in intakeGroups[i]) {
                            intake.IncorrectFeedback();
                        }
                    }
                    totalAns += ans;
                }
                break;
            case Subject.Subtraction:
                for (int i = 0; i < targetPowerLvs.Count; i++)
                {
                    int ans = 0;
                    foreach (FoundryIntakeManager intake in intakeGroups[i])
                    {
                        ans = ans * 10 + intake.GetPower();
                    }

                    //if this set of intakes is correct, give feedback
                    bool match = false;
                    foreach (int power in targetPowerLvs){
                        if (ans == power){
                            foreach (FoundryIntakeManager intake in intakeGroups[i]) {
                                intake.CorrectFeedback();
                            }
                            match = true;
                        }
                    } 
                    //if not, give incorrect feedback
                    if(!match){
                        foreach (FoundryIntakeManager intake in intakeGroups[i]) {
                            intake.IncorrectFeedback();
                        }
                    }

                    if (i == 0) { totalAns = ans; }
                    else { totalAns -= ans; }
                }
                break;
            case Subject.Multiplication:
                totalAns = 1;
                for (int i = 0; i < targetPowerLvs.Count; i++)
                {
                    int ans = 0;
                    foreach (FoundryIntakeManager intake in intakeGroups[i])
                    {
                        ans = ans * 10 + intake.GetPower();
                    }

                    //if this set of intakes is correct, give feedback
                    bool match = false;
                    foreach (int power in targetPowerLvs){
                        if (ans == power){
                            foreach (FoundryIntakeManager intake in intakeGroups[i]) {
                                intake.CorrectFeedback();
                            }
                            match = true;
                        }
                    } 
                    //if not, give incorrect feedback
                    if(!match){
                        foreach (FoundryIntakeManager intake in intakeGroups[i]) {
                            intake.IncorrectFeedback();
                        }
                    }

                    totalAns *= ans;
                }
                break;
        }

        if (totalAns == windowQuestion.GetSolution())
        {
            Debug.Log("Forge correct weapon");

            tutorial.IntakeCorrectlySubmitted();

            // Give correct feedback if answer was reached through a different method than foundry was looking for
            for(int i = 0; i < intakeGroups.Length; i++){
                foreach (FoundryIntakeManager intake in intakeGroups[i]) {
                    intake.CorrectFeedback();
                }
            }
                            
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");

            foreach (GameObject enemy in enemies) {
                Spawner spawner = enemy.GetComponent<Spawner>();
                if (spawner) {
                    spawner.StartSpawns();
                }
            }

            OnWeaponForged.Invoke();
        }
        else
        {
            Debug.Log("Forge wrong weapon");
            
        }
    }
}
