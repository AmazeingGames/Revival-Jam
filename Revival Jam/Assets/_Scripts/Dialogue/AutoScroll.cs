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
    AutoScrollType currentScroll;

    enum AutoScrollType {  Auto, Times, None  };
    enum ScrollType { JumpTo, SmoothScroll }

    [SerializeField] ScrollRect scrollRect;

    [Header("Settings")]
    [SerializeField] ScrollType scrollType;
    [SerializeField] float scrollVelocity;
    [SerializeField] float maxScrollThreshold = .1f;

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
        if (!ShouldAutoScroll())
            StopAutoScroll();

        ResumeScroll();
    }

    int scrollTimes;

    bool ShouldAutoScroll()
    {
        if (Input.mouseScrollDelta.y >= 1)
        {
            Debug.Log("Player scrolled up");
            return false;
        }

        isMaxScroll = IsMaxScroll();
        if (!isMaxScroll)
        {
            Debug.Log("Not max scroll");
            return false;
        }

        if (currentScroll == AutoScrollType.None)
            return false;
        else if (currentScroll == AutoScrollType.Times && scrollTimes <= 0)
            return false;

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

    bool IsMaxScroll()
        => scrollRect.verticalNormalizedPosition < maxScrollThreshold;

    public void StartAutoScroll()
        => currentScroll = AutoScrollType.Auto;

    public void StartTimedScroll(int times)
    {
        currentScroll = AutoScrollType.Times;
        scrollTimes = times;
    }

    public void StopAutoScroll()
    {
        Debug.Log("stopped auto scroll");
        scrollRect.velocity = Vector2.zero;
        currentScroll = AutoScrollType.None;
    }
}
