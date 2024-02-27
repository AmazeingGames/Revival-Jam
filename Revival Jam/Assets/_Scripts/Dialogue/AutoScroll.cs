using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;
using static DialogueManager.DialogueEventArgs;
using static DialogueManager;

//Helper class to the TerminalManager
//Purpose is to start auto-scrolling down while the notes are playing
//Stops auto-scrolling when the player manually starts scrolling
public class AutoScroll : MonoBehaviour
{
    bool didPlayerScroll;
    bool isMaxScroll;
    CurrentScroll currentScroll;

    enum CurrentScroll {  Auto, Times, None  };
    enum ScrollType { JumpTo, SmoothScroll }

    [SerializeField] ScrollRect scrollRect;

    [Header("Settings")]
    [SerializeField] ScrollType scrollType;
    [SerializeField] float scrollVelocity;
    [SerializeField] float maxScrollThreshold = .1f;
    [SerializeField] float resumeScrollCheckDelay;
    //float resumeScrollTimer;

    private void OnEnable()
        => RaiseDialogue += HandleDialogue;

    private void OnDisable()
        => RaiseDialogue -= HandleDialogue;

    bool isDialoguePlaying;

    //Readies player Input on Note end
    void HandleDialogue(object sender, DialogueEventArgs dialogueEventArgs)
    {
        isDialoguePlaying = dialogueEventArgs.dialogueState == DialogueState.Entering;

        if (isDialoguePlaying)
            StartAutoScroll();
        else
            StopAutoScroll();
    }

    private void Update()
    {
        if (!Scroll())
            StopAutoScroll();

        ResumeScroll();
    }

    int scrollTimes;

    bool Scroll()
    {
        //Stop auto scroll checks
        didPlayerScroll = DidPlayerScroll();
        if (didPlayerScroll)
            return false;

        isMaxScroll = IsMaxScroll();
        if (!isMaxScroll)
            return false;

        if (currentScroll == CurrentScroll.None)
            return false;

        else if (currentScroll == CurrentScroll.Times && scrollTimes <= 0)
            return false;

        //Scroll down
        scrollTimes--;
        scrollRect.velocity = new Vector2(0, scrollVelocity);

        return true;
    }

    bool ResumeScroll()
    {
        //resumeScrollTimer -= Time.deltaTime;

        if (Input.mouseScrollDelta.y >= 0)
            return false;

        if (!isDialoguePlaying)
            return false;

        if (!isMaxScroll)
            return false;

        Debug.Log("RESUME SCROLL");
        StartAutoScroll();
        return true;
    }

    bool DidPlayerScroll()
    {
        if (Input.mouseScrollDelta.y != 0)
            return true;
        return false;
    }

    bool IsMaxScroll()
    {
        if (scrollRect.verticalNormalizedPosition < maxScrollThreshold)
            return true;
        return false;
    }

    public void StartAutoScroll()
    {
        currentScroll = CurrentScroll.Auto;
    }

    public void ScrollTimes(int times)
    {
        currentScroll = CurrentScroll.Times;
        scrollTimes = times;
    }

    public void StopAutoScroll()
    {
        Debug.Log("stopped auto scroll");
        scrollRect.velocity = Vector2.zero;
        currentScroll = CurrentScroll.None;
        //resumeScrollTimer = resumeScrollCheckDelay;
    }
}
