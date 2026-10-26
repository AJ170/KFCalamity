using System.Collections;
using UnityEngine;

public class FS_AddSome : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(MakeSomeNewOnes());
	}

	private IEnumerator MakeSomeNewOnes()
	{
		while (true)
		{
			GameObject go = (GameObject)Object.Instantiate(base.gameObject);
			Object.Destroy(go.GetComponent<FS_AddSome>());
			yield return new WaitForSeconds(1f);
		}
	}
}
