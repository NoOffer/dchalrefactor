using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class AssessmentTerminalManager : BaseInteractable
{
    [SerializeField] private GameObject surveillanceCam;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject separatorWall;

    private GameObject playerRef;

    // Start is called before the first frame update
    void Awake()
    {
        playerRef = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnInteract()
    {
        StartCoroutine("StartAssessmentChallenge");
    }

    public void OnQuitAssessmentChallenge()
    {
        StartCoroutine("QuitAssessmentChallenge");
    }

    IEnumerator StartAssessmentChallenge()
    {
        TransitionUIManager._instance.StartTransition();
        yield return new WaitForSeconds(TransitionUIManager._instance.GetStartTransitionSpan());

        playerRef.SetActive(false);
        surveillanceCam.SetActive(true);

        Cursor.lockState = CursorLockMode.None;

        TransitionUIManager._instance.EndTransition();
        yield return new WaitForSeconds(TransitionUIManager._instance.GetEndTransitionSpan());
    }

    IEnumerator QuitAssessmentChallenge()
    {
        TransitionUIManager._instance.StartTransition();
        yield return new WaitForSeconds(TransitionUIManager._instance.GetStartTransitionSpan());

        surveillanceCam.SetActive(false);
        playerRef.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;

        separatorWall.SetActive(false);

        TransitionUIManager._instance.EndTransition();
        yield return new WaitForSeconds(TransitionUIManager._instance.GetEndTransitionSpan());

    }

    public void OnSubmit()
    {
        int ans = Int32.Parse(inputField.text);

        // TODO: Communicate with WindowQuestion
        // Correct answer
        if (true)
        {
            StartCoroutine("QuitAssessmentChallenge");
        }
        // Incorrect answer
        else
        {

        }
    }
}