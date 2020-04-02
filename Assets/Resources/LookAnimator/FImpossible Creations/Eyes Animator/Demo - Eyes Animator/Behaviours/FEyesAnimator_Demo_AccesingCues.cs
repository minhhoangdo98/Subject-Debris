using FIMSpace.FEyes;
using UnityEngine;
using UnityEngine.UI;

public class FEyesAnimator_Demo_AccesingCues : MonoBehaviour
{
    public FEyesAnimator EyesAnimator;
    public RectTransform Thinking;
    public Text Response;
    public CanvasGroup Fade;
    public AudioSource ASource;

    private FEyesAnimator.FERandomMovementType lastAsk;
    private float askingTimer = 0f;
    private float askDelay = 0f;

    public FEyesAnimator_Demo_AccesingCuesClips[] Voices;
    private FEyesAnimator_Demo_AccesingCuesClips choosed;

    private void Start()
    {
        choosed = Voices[Random.Range(0, Voices.Length)];
    }

    void Update()
    {
        askDelay -= Time.deltaTime;

        if (askDelay < 0f)
        {
            if (askingTimer > 0f)
            {
                if (!Thinking.gameObject.activeInHierarchy)
                {
                    EyesAnimator.RandomMovementPreset = lastAsk;
                    Thinking.gameObject.SetActive(true);
                }
            }

            askingTimer -= Time.deltaTime;

            if (askingTimer < 0.5f)
            {
                EyesAnimator.RandomMovementPreset = FEyesAnimator.FERandomMovementType.Listening;
            }

            if (askingTimer < 0f)
            {
                if (Thinking.gameObject.activeInHierarchy)
                {
                    ASource.clip = GetClip();
                    ASource.PlayDelayed(Random.Range(0.1f, 0.2f));
                    Thinking.gameObject.SetActive(false);

                    askDelay = 1f;

                    Response.text = GetText();
                    Fade.alpha = 1f;
                }
            }
        }
    }

    public void AskImagineAudio() { QuestionAsked(FEyesAnimator.FERandomMovementType.AccessingImaginedAuditory); }
    public void AskMemoryAudio() { QuestionAsked(FEyesAnimator.FERandomMovementType.AccessingAuditoryMemory); }
    public void AskVisualMemory() { QuestionAsked(FEyesAnimator.FERandomMovementType.AccessingVisualMemory); }
    public void AskImagineVisual() { QuestionAsked(FEyesAnimator.FERandomMovementType.AccessingImaginedVisual); }
    public void AskSelfTalk() { QuestionAsked(FEyesAnimator.FERandomMovementType.AccessingInternalSelfTalk); }
    public void AskFeeling() { QuestionAsked(FEyesAnimator.FERandomMovementType.AccessingFeelings); }


    public void QuestionAsked(FIMSpace.FEyes.FEyesAnimator.FERandomMovementType type)
    {
        if (Fade.alpha < 1f) return;

        lastAsk = type;
        askDelay = Random.Range(0.5f, 0.7f);
        askingTimer = Random.Range(3f, 5f);
        Fade.alpha = 0.5f;

        ASource.clip = choosed.Think[Random.Range(0, choosed.Think.Length)];
        ASource.PlayDelayed(Random.Range(0.2f, 0.4f));
    }

    private string GetText()
    {
        switch (lastAsk)
        {
            case FEyesAnimator.FERandomMovementType.AccessingImaginedVisual: return "Yeah! I think it would look pretty cool!";
            case FEyesAnimator.FERandomMovementType.AccessingImaginedAuditory: return "Hm, maybe something like that? Ouh...";
            case FEyesAnimator.FERandomMovementType.AccessingFeelings: return "Ouh first steps would burn my toes!";
            case FEyesAnimator.FERandomMovementType.AccessingVisualMemory: return "It was green.";
            case FEyesAnimator.FERandomMovementType.AccessingAuditoryMemory: return "Like broken refrigerator :O";
            case FEyesAnimator.FERandomMovementType.AccessingInternalSelfTalk: return "Done.";
        }

        return "";
    }

    private AudioClip GetClip()
    {
        switch (lastAsk)
        {
            case FEyesAnimator.FERandomMovementType.AccessingImaginedVisual: return choosed.ImagVisual;
            case FEyesAnimator.FERandomMovementType.AccessingImaginedAuditory: return choosed.ImagSound;
            case FEyesAnimator.FERandomMovementType.AccessingFeelings: return choosed.Feeling;
            case FEyesAnimator.FERandomMovementType.AccessingVisualMemory: return choosed.MemoVisual;
            case FEyesAnimator.FERandomMovementType.AccessingAuditoryMemory: return choosed.MemoSound;
            case FEyesAnimator.FERandomMovementType.AccessingInternalSelfTalk: return choosed.SelfTalk;
        }

        return null;
    }
}
