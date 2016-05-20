using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class Location{

	public Location(int pageNum, Vector3 position3D, int index){
		this.pageNum = pageNum;
		this.position3D = position3D;
		this.index = index;
	}

	public int pageNum;//所在的page
	public Vector3 position3D;//在page中的位置
	public int index;//在所有app中的索引

	public override string ToString ()
	{
		string str = "pageNum:"+pageNum+" position3D:"+position3D+" index:"+index;
		return str;
	}
}

public class IconController : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

	const string TAG = "IconController";

	public App app;

	DragMe dragMe;
	LauncherController mLauncherController;
	IconDragController mIconDragController;
	LauncherModel launcherModel;
	GameObject selectedBg;

	public Location originLocation;
	public Location regressLocation;

	void Awake(){
		mLauncherController = GameObject.FindGameObjectWithTag("GameController").GetComponent<LauncherController>();
		mIconDragController = mLauncherController.iconDragController;
		launcherModel = mLauncherController.launcherModel;
	}

	void Start(){
		var group = gameObject.AddComponent<CanvasGroup>();
		group.blocksRaycasts = false;
	}

	public void hideText(){
		Text appName = GetComponentInChildren<Text> ();
		appName.text = "";
	}

	public void showText(){
		Text appName = GetComponentInChildren<Text> ();
		appName.text = app.appName;
	}

	public void MoveToLocation(Location toLocation){
		RectTransform rt = GetComponent<RectTransform> ();
		if (originLocation.pageNum != toLocation.pageNum) {
			if (originLocation.index != mIconDragController.getDragIconLocation ().index) {
				this.regressLocation = originLocation;
				GameObject pageLayout =  mLauncherController.pageList [toLocation.pageNum];
				transform.SetParent (pageLayout.transform);
			}
		}
		this.originLocation = toLocation;
		rt.DOAnchorPos3D (originLocation.position3D, LauncherController.ANIM_DURATION);
	}

	public void SetOriLocation(Location toLocation){
		this.originLocation = toLocation;
	}

	public void LocationRegress(){
		if (regressLocation == null) {
			return;
		}
		this.originLocation = regressLocation;
		regressLocation = null;
		GameObject pageLayout =  mLauncherController.pageList [originLocation.pageNum];
		transform.SetParent (pageLayout.transform);
		RectTransform rt = GetComponent<RectTransform> ();
		rt.DOAnchorPos3D (originLocation.position3D, LauncherController.ANIM_DURATION);
	}

	public void LocationConfirm(bool anim){
		GameObject pageLayout =  mLauncherController.pageList [originLocation.pageNum];
		transform.SetParent (pageLayout.transform);
		RectTransform rt = GetComponent<RectTransform> ();
		if (anim) {
			rt.DOAnchorPos3D (originLocation.position3D, LauncherController.ANIM_DURATION);
		} else {
			rt.anchoredPosition3D = originLocation.position3D;
		}
	}

	public void DestorySelt(bool anim){
		RectTransform rt = GetComponent<RectTransform> ();
		if (anim) {
			Tweener tweener = rt.DOScale (0f, LauncherController.ANIM_DURATION);
			tweener.OnComplete (DestorySelt);
		} else {
			DestorySelt ();
		}


	}

	void DestorySelt(){
		transform.SetParent (null);
		Destroy (gameObject);
	}

	public void DestoryIcon(){
		GameObject icon = GameUtils.GetChildByName (gameObject, "Icon");
		icon.transform.DOScale (0, LauncherController.ANIM_DURATION);
	}

	public App getApp(){
		return this.app;
	}

	public void setApp(App app){
		this.app = app;
		GameObject icon = transform.GetChild (0).gameObject;
		RectTransform iconRt = icon.GetComponent<RectTransform> ();
		iconRt.sizeDelta = new Vector2 (launcherModel.iconWidth, launcherModel.iconWidth);
		GameObject lable = icon.transform.GetChild (0).gameObject;
		if (!app.isPlaceholder) {
			icon.SetActive (true);
			//				Debug.Log ("!app.isPlaceholder");
			if (app.iconUrl != null && app.iconUrl.Length > 0) {
				//					Log (TAG, "app.iconUrl:"+app.iconUrl);
				Sprite sprite = mLauncherController.LoadImgByIO (app);
				Image img = icon.GetComponent<Image> ();
				img.sprite = sprite;
			}
			Text textLable = lable.GetComponent<Text> ();
			textLable.fontSize = launcherModel.fontSize;
			textLable.text = app.appName;
		} else {
			//				Debug.Log ("app.isPlaceholder");
			icon.SetActive (false);
		}
	}

	public void ShowSelectedBackground(){
		selectedBg = new GameObject ("selected bg");
		selectedBg.transform.SetParent (gameObject.transform);
		selectedBg.transform.SetSiblingIndex (0);
		selectedBg.transform.localScale = Vector3.one;
		RectTransform rt = selectedBg.AddComponent<RectTransform> ();
		rt.sizeDelta = GetComponent<RectTransform> ().sizeDelta;
		rt.anchoredPosition3D = Vector3.zero;
		Image image = selectedBg.AddComponent<Image> ();
		Color color = new Color(0.22f, 0.22f, 0.22f, 0.5f);
		image.color = color;
	}

	public void DestroySelectedBackground(){
		if(selectedBg!=null){
			Destroy (selectedBg);
			selectedBg = null;
		}
	}

	public void OnLongClick (PointerEventData lastEventData)
	{
		mLauncherController.Log (TAG, "OnLongClick: lastEventData.position="+lastEventData.position);
		DestroySelectedBackground ();
		RectTransform rt = gameObject.GetComponent<RectTransform> ();
		float posZ = LauncherController.ICON_RISING_DISTANCE;
		mLauncherController.Log (TAG, "OnLongClick: AnchorPos3D="+rt.anchoredPosition3D);
		Vector3 toPos = new Vector3 (rt.anchoredPosition3D.x, rt.anchoredPosition3D.y, posZ);
		Tweener tw = rt.DOAnchorPos3D (toPos, LauncherController.ANIM_DURATION);
		tw.OnComplete (test);
		dragMe = gameObject.AddComponent<DragMe> ();
		dragMe.mLauncherController = mLauncherController;
		dragMe.launcherModel = mLauncherController.launcherModel;
		dragMe.OnPointerDown (lastEventData);

		TrashController trashController = mLauncherController.trashRange.GetComponent<TrashController> ();
		trashController.Rise ();
	}

	void test(){
		RectTransform rt = gameObject.GetComponent<RectTransform> ();
		mLauncherController.Log (TAG, "OnLongClick test: AnchorPos3D="+rt.anchoredPosition3D);
	}


		
	#region IPointerDownHandler implementation

	public void OnPointerDown (PointerEventData eventData)
	{
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
	}

	#endregion

	#region IPointerUpHandler implementation

	public void OnPointerUp (PointerEventData eventData)
	{
		TrashController trashController = mLauncherController.trashRange.GetComponent<TrashController> ();
		trashController.OnDrop (eventData);
	}

	#endregion

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		if (dragMe != null) {
			dragMe.OnBeginDrag (eventData);
		} 
			
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		if (dragMe != null) {
			dragMe.OnDrag (eventData);
		} 
		NotifyTrash ();
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		if (dragMe != null) {
//			Debug.Log ("dragme OnEndDrag");
			GameObject pageLayout =  mLauncherController.pageList [originLocation.pageNum];
			transform.SetParent (pageLayout.transform);
			dragMe.OnEndDrag (eventData);
			RectTransform rt = GetComponent<RectTransform> ();
			Tweener tweener = rt.DOAnchorPos3D (originLocation.position3D, LauncherController.ANIM_DURATION);
			tweener.OnComplete (DestroyDragMe);

		} 
	}

	#endregion

	void DestroyDragMe(){
		Destroy (dragMe);
	}

	void NotifyTrash(){
		GameObject trashRange = mLauncherController.trashRange;
		TrashController trashListener = trashRange.GetComponent<TrashController> ();
		trashListener.OnIconDrag ();
	}
}
