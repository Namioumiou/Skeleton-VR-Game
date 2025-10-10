using UnityEngine;

using UnityEngine;
using TMPro;
using System.Collections;

public class StoryController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text storyText;

    [Header("Story")]
    [TextArea(2, 5)] public string[] storyLines;

    [Header("Timing")]
    [Tooltip("Seconds between characters while typing")]
    public float charInterval = 0.03f;
    [Tooltip("Seconds to keep a fully-typed line on screen before advancing")]
    public float holdAfterLine = 2.0f;
    [Tooltip("Seconds to wait after the last line before hiding text")]
    public float endHoldTime = 2.5f;
    [Tooltip("Fade out duration for the last line")]
    public float fadeOutDuration = 1.5f;


    [Tooltip("Optional per-line hold overrides (same length as storyLines). Leave empty to use holdAfterLine for all.")]
    public float[] perLineHoldSeconds;

    [Header("Flow")]
    public bool playOnStart = true;
    public bool loop = false;

    int index = 0;
    bool isTyping = false;
    Coroutine runner;
    CanvasGroup canvasGroup;

    
    void Awake()
    {
        if (storyText != null)
        {
            canvasGroup = storyText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = storyText.gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Start()
    {
        if (storyText) storyText.text = "";
        if (playOnStart) Play();
    }

    public void Play()
    {
        Stop();
        runner = StartCoroutine(RunStory());
    }

    public void Stop()
    {
        if (runner != null) StopCoroutine(runner);
        runner = null;
        isTyping = false;
    }

    IEnumerator RunStory()
    {
        if (!storyText || storyLines == null || storyLines.Length == 0)
            yield break;

        index = Mathf.Clamp(index, 0, storyLines.Length - 1);

        while (true)
        {
            // Type current line
            yield return StartCoroutine(TypeLine(storyLines[index]));

            // Hold on the fully-typed line
            float hold = holdAfterLine;
            if (perLineHoldSeconds != null && perLineHoldSeconds.Length == storyLines.Length)
                hold = perLineHoldSeconds[index];

            yield return new WaitForSeconds(hold);

            // Advance
            index++;

            // End / loop
            if (index >= storyLines.Length)
            {
                if (loop) index = 0;
                else break;
            }
        }

        yield return new WaitForSeconds(endHoldTime);
        yield return StartCoroutine(FadeOut());
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        storyText.text = "";

        if (charInterval <= 0f) // instant mode if interval is zero
        {
            storyText.text = line;
        }
        else
        {
            foreach (char c in line)
            {
                storyText.text += c;
                yield return new WaitForSeconds(charInterval);
            }
        }

        isTyping = false;
    }

    // Optional: call this from elsewhere to jump to next line instantly.
    public void SkipTyping()
    {
        if (!isTyping) return;
        StopAllCoroutines();
        storyText.text = storyLines[index];
        isTyping = false;
        runner = StartCoroutine(ResumeAfterSkip());
    }

    IEnumerator ResumeAfterSkip()
    {
        float hold = (perLineHoldSeconds != null && perLineHoldSeconds.Length == storyLines.Length)
            ? perLineHoldSeconds[index]
            : holdAfterLine;
        yield return new WaitForSeconds(hold);
        index++;
        if (index < storyLines.Length || loop) runner = StartCoroutine(RunStory());
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeOutDuration);
            yield return null;
        }

        storyText.gameObject.SetActive(false);
    }
}

