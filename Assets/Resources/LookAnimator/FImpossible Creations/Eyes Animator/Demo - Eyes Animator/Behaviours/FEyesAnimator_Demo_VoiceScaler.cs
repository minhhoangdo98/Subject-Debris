using UnityEngine;

public class FEyesAnimator_Demo_VoiceScaler : MonoBehaviour
{
    public AudioSource targetSource;

	void Update ()
    {
        if (!targetSource.isPlaying)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0f, 0.1f, 0.05f), Time.deltaTime * 7f);
        }
        else
        {
            float[] samples = new float[32];
            float sourceTimeLookAhead = targetSource.time;

            if (sourceTimeLookAhead > targetSource.clip.length)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0f, 0.1f, 0.05f), Time.deltaTime * 7f);
            }
            else
            {
                targetSource.clip.GetData(samples, (int)(targetSource.clip.samples * (sourceTimeLookAhead / targetSource.clip.length)));

                float sum = 0f;
                for (int x = 0; x < samples.Length; x++) sum += samples[x] * samples[x];
                sum = Mathf.Sqrt(sum);

                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(sum, 0.01f, sum / 2), Time.deltaTime * 7f);
            }
        }
    }
}
