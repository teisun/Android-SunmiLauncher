using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class PageLayout : MonoBehaviour {

	const string TAG = "PageLayout";
	const float moveBalance = 10f;

	public IFlipTween flipTween;
	private LauncherController mLauncherController;
	private LauncherModel launcherModel;

	public int pageNumber = -1;
	private LongClickUtils longClickUtils;
	private GameObject currentSelectedIcon;

	void Awake(){
		mLauncherController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<LauncherController>();
		launcherModel = mLauncherController.launcherModel;
	}

	public void setUITransform(Vector3 pos3D){
		RectTransform rt = GetComponent<RectTransform> ();
		rt.anchoredPosition3D = pos3D;
		rt.sizeDelta = new Vector2 (launcherModel.screenWidth, launcherModel.screenHeight);
//		Debug.Log ("setUITransform:"+rt.sizeDelta);
		gameObject.AddComponent<CutCardsTween> ();
		flipTween = GetComponent<CutCardsTween> ();
	}

	public void addChild(GameObject child, Vector3 pos3D, Vector2 sizeDelta){
		child.transform.SetParent (transform);
		child.transform.localScale = Vector3.one;
		RectTransform rt = child.GetComponent<RectTransform> ();
		rt.anchoredPosition3D = pos3D;
		rt.sizeDelta = sizeDelta;
	}


	public int getLinkagePosition(){
		return flipTween.GetLinkagePosition ();
	}

	public void Entry(){
		flipTween.Entry ();
	}
		

}
