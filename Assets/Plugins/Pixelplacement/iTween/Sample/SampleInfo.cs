using UnityEngine;
using System.Collections;

public class SampleInfo : MonoBehaviour
{
void OnGUI(){
		GUILayout.Label("iTween can spin, shake, punch, move, handle audio, fade color and transparency \nand much more with each task needing only one line of code.");
		GUILayout.BeginHorizontal();
		GUILayout.Label("iTween works with C#, JavaScript and Boo. For full documentation and examples visit:");
		if(GUILayout.Button("http://itween.pixelplacement.com")){
			Application.OpenURL("http://itween.pixelplacement.com");
		}
		GUILayout.EndHorizontal();
	}
    private void Start()
    {
        StartCoroutine(ShakeCam());   
    }
    IEnumerator ShakeCam()
    {
        yield return new WaitForSeconds(1f);
        iTween.ShakePosition(gameObject, new Vector3(0.1f, 0.1f, 0.1f), 0.5f);
        yield return new WaitForSeconds(1);
        iTween.ShakePosition(gameObject, new Vector3(0.1f, 0.1f, 0.1f), 0.5f);
        yield return new WaitForSeconds(1);
        iTween.ShakePosition(gameObject, new Vector3(0.1f, 0.1f, 0.1f), 0.5f);
        yield return new WaitForSeconds(1);
        iTween.ShakePosition(gameObject, new Vector3(0.1f, 0.1f, 0.1f), 0.5f);
        yield return new WaitForSeconds(1);
        iTween.ShakePosition(gameObject, new Vector3(0.1f, 0.1f, 0.1f), 0.5f);
        yield return new WaitForSeconds(1);
        iTween.ShakePosition(gameObject, new Vector3(0.1f, 0.1f, 0.1f), 0.5f);
        yield return new WaitForSeconds(1);
        iTween.ShakePosition(gameObject, new Vector3(0.1f, 0.1f, 0.1f), 0.5f);
    }
}

