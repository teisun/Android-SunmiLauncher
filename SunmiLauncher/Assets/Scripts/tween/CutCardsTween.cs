using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class CutCardsTween : MonoBehaviour, IFlipTween {

	const string TAG = "IFlipTween";

	private Vector3 orginPosition;
	private RectTransform rt;
	private float duration;
	private PageLayout pageLayout;
	private int linkagePosition = -1;
	private PageController mPageController;
	private float posY;

	private LauncherController mLauncherController;

	void Awake () {
		rt = GetComponent<RectTransform> ();
		duration = LauncherController.ANIM_DURATION;
		pageLayout = GetComponent<PageLayout> ();
		mPageController = GameObject.FindGameObjectWithTag ("PageController").GetComponent<PageController> ();
		posY = 0f;
		orginPosition =  rt.anchoredPosition3D;
		mLauncherController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<LauncherController> ();
		mLauncherController.Log (TAG, "CutCardsTween Awake() pageNumber:"+pageLayout.pageNumber+" OrginPosition:"+orginPosition.ToString());
	}


	#region IFlipTween implementation

	public void Reset(){
		rt.DOAnchorPos3D (orginPosition, LauncherController.ANIM_DURATION);
		mLauncherController.Log (TAG, "Reset pageNumber:"+pageLayout.pageNumber+" OrginPosition:"+orginPosition+" anchoredPosition3D:"+rt.anchoredPosition3D);
		linkagePosition = -1;
	}

	public void SetOrginPosition (Vector3 pos3D)
	{
		orginPosition = pos3D;
	}

	public Vector3 GetOrginPosition(){
		return orginPosition;
	}


	public void LeftSlide (float offsetX)
	{
		Vector3 pos3D =  rt.anchoredPosition3D;
		if (pos3D.x <= 0f) {
			float localPosX = rt.anchoredPosition3D.x;
			float localPosZ = rt.anchoredPosition3D.z - offsetX;
			if (localPosZ < 0f) {
				localPosZ = 0f;
			}
			rt.anchoredPosition3D = new Vector3 (localPosX, posY, localPosZ);
			linkagePosition = pageLayout.pageNumber + 1;
//			Debug.Log ("pos3D.x <= 0f");
		}else if(pos3D.x > 0f){
			float localPosX = rt.anchoredPosition3D.x + offsetX;
			float localPosZ = rt.anchoredPosition3D.z;
			rt.anchoredPosition3D = new Vector3 (localPosX, posY, localPosZ);
			linkagePosition = pageLayout.pageNumber - 1;
//			Debug.Log ("pos3D.x > 0f");
		}
	}

	public void RightSlide (float offsetX)
	{
		Vector3 pos3D =  rt.anchoredPosition3D;
		if (pos3D.z > 0) {
			float localPosX = rt.anchoredPosition3D.x;
			float localPosZ = rt.anchoredPosition3D.z - offsetX;
			if (localPosZ < 0f) {
				localPosZ = 0f;
			}
			rt.anchoredPosition3D = new Vector3 (localPosX, posY, localPosZ);
			linkagePosition = pageLayout.pageNumber + 1;
		} else{
			float localPosX = rt.anchoredPosition3D.x + offsetX;
			float localPosZ = rt.anchoredPosition3D.z;
			if (localPosZ < 0f) {
				localPosZ = 0f;
			}
			rt.anchoredPosition3D = new Vector3 (localPosX, posY, localPosZ);
			linkagePosition = pageLayout.pageNumber - 1;
		}

	}

	public void LeftExit ()
	{
		
		float toZ = Screen.width;
		Vector3 pos3D = new Vector3 (0f, posY, toZ);
		SetOrginPosition (pos3D);
		Tweener tweener = rt.DOAnchorPos3D (pos3D, duration);
		tweener.OnComplete (Reset);
		mLauncherController.Log (TAG, "LeftExit pageNumber:"+pageLayout.pageNumber);
	}

	public void LeftEnter ()
	{
		Vector3 pos3D = new Vector3(0f,posY,0f);
		SetOrginPosition (pos3D);
		Tweener tweener = rt.DOAnchorPos3D (pos3D, duration);
		tweener.OnComplete (Reset);
		mLauncherController.Log (TAG, "LeftEnter pageNumber:"+pageLayout.pageNumber);
	}

	public void RightExit ()
	{
		Vector3 pos3D = new Vector3(Screen.width,posY,0f);
		SetOrginPosition (pos3D);
		Tweener tweener = rt.DOAnchorPosX (Screen.width, duration);
		tweener.OnComplete (Reset);
	}

	public void RightEnter ()
	{
		Vector3 pos3D = new Vector3(0f,posY,0f);
		SetOrginPosition (pos3D);
		Tweener tweener = rt.DOAnchorPos3D (pos3D, duration);
		tweener.OnComplete (Reset);
		mLauncherController.Log (TAG, "RightEnter pageNumber:"+pageLayout.pageNumber);
	}

	public void LeftReset ()
	{
		int pageSize = mPageController.pageSize;
		int position = pageLayout.pageNumber;
		if (position == pageSize - 1) {
			SetOrginPosition (new Vector3(0f, posY, 0f));
		}
		mLauncherController.Log (TAG, "LeftReset position:"+pageLayout.pageNumber+" OrginPosition:"+orginPosition.ToString());
		Tweener tweener = rt.DOAnchorPos3D (orginPosition, duration);
		tweener.OnComplete (Reset);
	}

	public void RightReset ()
	{
		int position = pageLayout.pageNumber;
		if (position == 0) {
			SetOrginPosition (new Vector3(0f, posY, 0f));
		}
		mLauncherController.Log (TAG, "RightReset position:"+pageLayout.pageNumber+" OrginPosition:"+orginPosition.ToString());
		Tweener tweener = rt.DOAnchorPosX (orginPosition.x, duration);
		tweener.OnComplete (Reset);
	}

	public int GetLinkagePosition(){
		return linkagePosition;
	}


	public void Entry ()
	{
		Vector3 pos3D = new Vector3(0f,posY,0f);
		SetOrginPosition (pos3D);
		Tweener tweener = rt.DOAnchorPos3D (pos3D, duration);
		tweener.OnComplete (Reset);
	}
	#endregion


}
