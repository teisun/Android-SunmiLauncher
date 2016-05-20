using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Pathfinding.Serialization.JsonFx;

public class TrashController : MonoBehaviour, IDropHandler
{
	const string TAG = "TrashController";
	const float ANIM_DURATION = 0.5f;

	LauncherController mLauncherController;
	IconDragController mIconDragController;
	Color oriColor ;
	bool controlFlag;
	App uninstallApp;

	void Start(){
		mLauncherController =  GameObject.FindGameObjectWithTag ("GameController").GetComponent<LauncherController>();
		mIconDragController = mLauncherController.iconDragController;
		mLauncherController.Log (TAG, "Start");
	}

	public void OnEnable ()
	{
	}
	
	public void OnDrop(PointerEventData data)
	{
		mLauncherController.Log (TAG, "OnDrop");
		Fall ();
		if (controlFlag) {
			controlFlag = false;
			GameObject originalObj = mIconDragController.currentDragIcon;
			IconController iconController = originalObj.GetComponent<IconController> ();
			iconController.showText ();
			uninstallApp = iconController.getApp ();
			AskForUninstall (uninstallApp.packageName);
		}

	}

	// android plugin method
	public void AskForUninstall(string packageName){
		AndroidJavaObject launcherPlugin = mLauncherController.launcherPlugin;
		string[] paramsArr = new string[3];
		paramsArr[0] = "TrashRange";
		paramsArr[1] = "MessageSilenceUninstall";
		paramsArr[2] = packageName;
		launcherPlugin.Call ("askForUninstall", paramsArr);
	}

	public void UninstallApp(App app){
		AndroidJavaObject launcherPlugin = mLauncherController.launcherPlugin;
		string json = JsonWriter.Serialize (app);
		mLauncherController.Log (TAG, "UninstallApp json:"+json);
		string[] paramsArr = new string[1];
		paramsArr[0] = json;
		launcherPlugin.Call ("uninstall", paramsArr);
		uninstallApp = null;
	}

	public void SilenceUninstallApp(App app){
		AndroidJavaObject launcherPlugin = mLauncherController.launcherPlugin;
		string json = JsonWriter.Serialize (app);
		mLauncherController.Log (TAG, "silenceUninstall json:"+json);
		string[] paramsArr = new string[1];
		paramsArr[0] = json;
		launcherPlugin.Call ("silenceUninstall", paramsArr);
		uninstallApp = null;
	}

	//----------------------------------------------------//

	// android call back
	public void MessageSilenceUninstall(string answer){
		if (answer.Equals ("true")) {
//			SilenceUninstallApp (uninstallApp);
			DialogController dialogController = mLauncherController.uninstallDialog.GetComponent<DialogController>();
			dialogController.show (uninstallApp);
		} else {
			UninstallApp (uninstallApp);
		}

	}

//	public void MessageUninstalled(string appJson){
//		mLauncherController.Log (TAG, "MessageUninstalled appJson:"+appJson);
//		App app = JsonReader.Deserialize<App> (appJson);
//		DialogController dialogController = mLauncherController.uninstallDialog.GetComponent<DialogController>();
//		dialogController.uninstalled ();
//		mLauncherController.OnUninstalled(app);
//	}

	//----------------------------------------------------//

	public void SilenceUninstallApp(){
		SilenceUninstallApp (uninstallApp);
		DialogController dialogController = mLauncherController.uninstallDialog.GetComponent<DialogController>();
		dialogController.progress ();
	}

	public bool OnIconDrag(){
		GameObject originalObj = mIconDragController.currentDragIcon;
		IconController iconController = originalObj.GetComponent<IconController> ();
		if (oriColor.Equals(Color.clear)) {
			Image bg = originalObj.GetComponent<Image> ();
			oriColor = bg.color;
		}
		bool enter = OnIconEnterOrExit ();
		if (enter) {
			if (!controlFlag) {
				controlFlag = true;
				mLauncherController.Log (TAG, "OnIconDrag1 enter"+enter);
				iconController.hideText ();
				DoIconAnim (originalObj, Color.clear);
			}
		} else {
			if (controlFlag) {
				controlFlag = false;
				mLauncherController.Log (TAG, "OnIconDrag2 enter"+enter);
				iconController.showText ();
				DoIconAnim (mIconDragController.currentDragIcon, oriColor);
			}
		}
		return enter;
	}


	void DoIconAnim(GameObject icon, Color endValue){
		if (icon != null) {
			Image bg = icon.GetComponent<Image> ();
			bg.DOColor (endValue, ANIM_DURATION);
		}
	}

	/// <summary>
	/// Raises the icon enter or exit event.
	/// </summary>
	bool OnIconEnterOrExit(){
		GameObject currentDragIcon = mIconDragController.currentDragIcon;
		RectTransform rt = currentDragIcon.GetComponent<RectTransform> ();
		Vector2 anchoredPosition = rt.anchoredPosition;
		Vector2 center = rt.rect.center;
		Vector2 iconCenter = new Vector2 (anchoredPosition.x+center.x, anchoredPosition.y+center.y);

		RectTransform rtTrashRange = GetComponent<RectTransform> ();
		Vector2 range = rtTrashRange.sizeDelta;

		if (iconCenter.x < range.x && iconCenter.y < range.y + rtTrashRange.anchoredPosition.y ) {
			return true;//center enter
		} else {
			return false;//center exit
		}
	}

	public void Rise(){
		LauncherModel launcherModel = mLauncherController.launcherModel;
		GameObject currentDragIcon = mIconDragController.currentDragIcon;
		gameObject.SetActive (true);
		transform.SetSiblingIndex (currentDragIcon.transform.GetSiblingIndex()-1);
		RectTransform rtTrashRange = GetComponent<RectTransform> ();
		rtTrashRange.DOAnchorPosY (-launcherModel.navigationBarHeight, ANIM_DURATION);
	}

	public void Fall(){
		mLauncherController.Log (TAG, "Fall");
		RectTransform rtTrashRange = GetComponent<RectTransform> ();
		Tweener tw = rtTrashRange.DOAnchorPosY (-rtTrashRange.sizeDelta.y, ANIM_DURATION);
		tw.OnComplete (SetActiveFalse);
		if (!oriColor.Equals (Color.clear)) {
			DoIconAnim (mIconDragController.currentDragIcon, oriColor);
		}

	}

	void SetActiveFalse(){
		gameObject.SetActive (false);
	}

}
