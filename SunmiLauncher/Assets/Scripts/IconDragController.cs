using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;

class SwapInfo{
	//drag page info
	public int dragPage;
	public int dragIndex;
	public App dragModel;
	public GameObject dragIcon;
	public Location dragLocation;
	public int lastIndexOnDragPage;
	public App lastModelOnDragPage;
	public GameObject lastIconOnDragPage;
	public Location lastLocationOnDragPage;

	public int firstEmptyIconIndexOnDragPage = -1;
	public App firstEmptyIconModelOnDragPage;
	public GameObject firstEmptyIconIconOnDragPage;
	public Location firstEmptyIconLocationOnDragPage;

	//drop page info
	public int dropPage;
	public int dropIndex;
	public App dropModel;
	public GameObject dropIcon;
	public Location dropLocation;
	public int lastIndexOnDropPage;
	public App lastModelOnDropPage;
	public GameObject lastIconOnDropPage;
	public Location lastLocationOnDropPage;

	public int firstEmptyIconIndexOnDropPage = -1;
	public App firstEmptyIconModelOnDropPage;
	public GameObject firstEmptyIconIconOnDropPage;
	public Location firstEmptyIconLocationOnDropPage;
}

//-------Icon drag controller------------//
public class IconDragController : MonoBehaviour {

	public class IconMovingInfo{
		public int dragIndex = -1;
		public int dropIndex = -1;
		public bool beginIconDrag = false;
	}

	const string TAG = "IconDragController";

	[HideInInspector]
	public GameObject currentDragIcon;

	private IconMovingInfo iconMovingInfo = new IconMovingInfo ();

	private LauncherController launcherController;
	private PageController pageController;

	private SwapInfo swapInfo = new SwapInfo();

	// Use this for initialization
	void Start () {
		launcherController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<LauncherController> ();
		pageController = launcherController.pageController;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Log(string tag, string msg){
		launcherController.Log (tag, msg);
	}

	public bool IsIconDragMode(){
		return currentDragIcon != null;
	}

	public GameObject getCurrentDragIcon(){
		return currentDragIcon;
	}

	public Location getDragIconLocation(){
		IconController control = currentDragIcon.GetComponent<IconController> ();
		return control.originLocation;
	}

	public void OnIconLongClick(PointerEventData eventData, GameObject page){

		List<GameObject> pageChildList = launcherController.pageChildList;

		int index = pageController.PointToIndex (eventData.position);
		if (index == -1) {
			return;
		}
//		Log (TAG, "OnIconLongClick eventData.position:"+eventData.position);
		currentDragIcon = pageChildList[index];
		IconController action = currentDragIcon.GetComponent<IconController> ();
		action.OnLongClick (eventData);
		iconMovingInfo.dragIndex = index;
	}

	public void OnIconPointerUp(PointerEventData eventData, GameObject page){
//		Log (TAG, "OnIconPointerUp eventData.position:"+eventData.position);
		IconController action = currentDragIcon.GetComponent<IconController> ();
		action.OnPointerUp (eventData);
		if (!iconMovingInfo.beginIconDrag) {
//			Log (TAG, "!iconMovingInfo.beginIconDrag");
			action.OnEndDrag (eventData);
			OnIconEndDrag (eventData, page);
		}
	}

	public void OnIconBeginDrag(PointerEventData eventData, GameObject page){
//		Log (TAG, "OnIconBeginDrag eventData.position:"+eventData.position);
		iconMovingInfo.beginIconDrag = true;
		IconController action = currentDragIcon.GetComponent<IconController> ();
		action.OnBeginDrag (eventData);
	}

	public void OnIconDrag(PointerEventData eventData, GameObject page){
//		Log (TAG, "OnIconDrag eventData.position:"+eventData.position);
//		Log (TAG, "OnIconDrag current pageIndex:"+pageController.getCurrentPosition()+" drag page:"+page.GetComponent<PageLayout>().pageNumber);
		IconController action = currentDragIcon.GetComponent<IconController> ();
		action.OnDrag (eventData);

		RectTransform rt = currentDragIcon.GetComponent<RectTransform> ();
		Vector2 anchoredPosition = rt.anchoredPosition;
		Vector2 center = rt.rect.center;
		Vector2 iconCenter = new Vector2 (anchoredPosition.x+center.x, anchoredPosition.y+center.y);
//		Log (TAG, "OnIconDrag iconCenter:"+iconCenter);
		int touchIndex = pageController.PointToIndex (iconCenter);
		//		Log (TAG, "OnIconDrag touchIndex:"+touchIndex);
		//		Log(TAG, "OnIconDrag dragIndex:"+iconMovingInfo.dragIndex+" dropIndex:"+iconMovingInfo.dropIndex);
		if (touchIndex == -1) {
			Log (TAG, "touchIndex == -1");
			return;
		}

		if (iconMovingInfo.dropIndex != touchIndex) {
			CancelInvoke ();
		}
		if (iconMovingInfo.dragIndex != touchIndex) {
			iconMovingInfo.dropIndex = touchIndex;
		}
		if (iconMovingInfo.dropIndex == -1) {
			return;
		}
		if (iconMovingInfo.dragIndex == iconMovingInfo.dropIndex) {
			return;
		}
		Invoke ("InvokeExchange" , 0.4f);
	}

	void InvokeExchange(){
		ChildExChange(iconMovingInfo.dragIndex, iconMovingInfo.dropIndex);
	}

	public void OnIconEndDrag(PointerEventData eventData, GameObject page){
//		Log (TAG, "OnIconEndDrag eventData.position:"+eventData.position);
		iconMovingInfo.beginIconDrag = false;
		CancelInvoke ();
		IconController action = currentDragIcon.GetComponent<IconController> ();
		action.OnEndDrag (eventData);
//		Log (TAG, "OnIconEndDrag currentDragIcon = null");
		currentDragIcon = null;
		if (iconMovingInfo.dragIndex != iconMovingInfo.dropIndex) {
			iconMovingInfo.dropIndex = iconMovingInfo.dragIndex;
		}

	}

	public void ChildExChange(int drag, int drop){
		if (drag == drop) {
			return;
		}

		int pageItemNum = launcherController.launcherModel.pageItemNum;
		int dragPage = drag / pageItemNum;
		int dropPage = drop / pageItemNum;
		ResetSwapInfo (drag, drop);
		if (dragPage == dropPage) {
			//same Page swap
//			launcherController.Log(TAG, "same Page swap");
			SamePageSwap (drag, drop);
		} else {
			//difference page swap
//			launcherController.Log(TAG, "difference page swap");
			DifferencePageSwap(drag, drop);
		} 

	}

	void DifferencePageSwap(int drag, int drop){
		if (swapInfo.dragPage > swapInfo.dropPage) {
			//left silde
			LeftSildeSwap();
		} else {
			//right silde
			RightSildeSwap();
		}
	}

	void RightSildeSwap(){
		launcherController.Log (TAG, "RightSildeSwap");
		LauncherModel launcherModel = launcherController.launcherModel;
		List<App> appList = launcherController.appList;
		List<GameObject> iconList = launcherController.pageChildList;

		int firstIndexOnDropPage = swapInfo.dropPage * launcherModel.pageItemNum;
		IconController iconController = iconList [firstIndexOnDropPage].GetComponent<IconController> ();
		if (iconController.regressLocation!=null) {
			//regress
			if (appList [swapInfo.dropIndex].isPlaceholder) {
				swapInfo.dropIndex = swapInfo.firstEmptyIconIndexOnDropPage - 1;
				swapInfo.dropModel = appList [swapInfo.dropIndex];
				swapInfo.dropIcon = iconList [swapInfo.dropIndex];
				swapInfo.dropLocation = swapInfo.dropIcon.GetComponent<IconController> ().originLocation;
			}
			//drag page icon 往前挪
			launcherController.IconAdvance (swapInfo.dragIndex, swapInfo.dragIndex + 1, swapInfo.dropIndex, true);
			appList.RemoveAt (swapInfo.dragIndex);
			iconList.RemoveAt (swapInfo.dragIndex);
			appList.Insert (swapInfo.dropIndex, swapInfo.dragModel);
			iconList.Insert (swapInfo.dropIndex, swapInfo.dragIcon);
		} else {
			//drag page icon 往前挪
			launcherController.IconAdvance (swapInfo.dragIndex, swapInfo.dragIndex+1, swapInfo.lastIndexOnDragPage, false);
			appList.RemoveAt (swapInfo.dragIndex);
			iconList.RemoveAt (swapInfo.dragIndex);
			//drag page 末尾插入一个占位icon
			launcherController.addPlaceholder (swapInfo.dragPage, swapInfo.lastIndexOnDragPage, swapInfo.lastLocationOnDragPage);

			if (appList [swapInfo.dropIndex].isPlaceholder) {
//				launcherController.Log (TAG, "RightSildeSwap DestoryFirstEmptyOnDropPageAndResetDropInfo");
				DestoryFirstEmptyOnDropPageAndResetDropInfo ();
			} else {
				//drop page icon 往后挪
//				launcherController.Log (TAG, "RightSildeSwap launcherController.IconBackward");
				launcherController.IconBackward(swapInfo.dropIndex, iconList.Count-1, true);
				appList.Insert (swapInfo.dropIndex, swapInfo.dragModel);
				iconList.Insert (swapInfo.dropIndex, swapInfo.dragIcon);
			}
		}
//		launcherController.Log (TAG, "RightSildeSwap SetOriLocation (swapInfo.dropLocation):"+swapInfo.dropLocation);
		swapInfo.dragIcon.GetComponent<IconController> ().SetOriLocation (swapInfo.dropLocation);
		iconMovingInfo.dragIndex = iconMovingInfo.dropIndex;

	}

	void LeftSildeSwap(){
		launcherController.Log (TAG, "LeftSildeSwap");
		LauncherModel launcherModel = launcherController.launcherModel;
		List<App> appList = launcherController.appList;
		List<GameObject> iconList = launcherController.pageChildList;

		if (swapInfo.lastModelOnDropPage.isPlaceholder) {//如果drop page末尾是空位

			//1.drag page icon 往前挪
			launcherController.IconAdvance (swapInfo.dragIndex, swapInfo.dragIndex+1, swapInfo.lastIndexOnDragPage, false);
			appList.RemoveAt (swapInfo.dragIndex);
			iconList.RemoveAt (swapInfo.dragIndex);
			//2.drag page 末尾插入一个占位icon
			launcherController.addPlaceholder (swapInfo.dragPage, swapInfo.lastIndexOnDragPage, swapInfo.lastLocationOnDragPage);

			//判断:如果drop index 是空位
			if(appList[swapInfo.dropIndex].isPlaceholder){
				//3.干掉第一个空位，将其Location赋予drag icon
				DestoryFirstEmptyOnDropPageAndResetDropInfo();

			}else{//drop index 不是空位
				//3.drop page icon 往后挪
				launcherController.IconBackward(swapInfo.dropIndex, swapInfo.lastIndexOnDropPage, true);
				appList.Insert (swapInfo.dropIndex, swapInfo.dragModel);
				iconList.Insert (swapInfo.dropIndex, swapInfo.dragIcon);
			}
		} else {
			//如果drop page末尾不是空位
			launcherController.IconBackward(swapInfo.dropIndex, swapInfo.dragIndex-1, true);
			appList.RemoveAt (swapInfo.dragIndex);
			iconList.RemoveAt (swapInfo.dragIndex);
			appList.Insert (swapInfo.dropIndex, swapInfo.dragModel);
			iconList.Insert (swapInfo.dropIndex, swapInfo.dragIcon);
		
		}
		swapInfo.dragIcon.GetComponent<IconController> ().SetOriLocation (swapInfo.dropLocation);
		iconMovingInfo.dragIndex = iconMovingInfo.dropIndex;

	}

	void SamePageSwap(int drag, int drop){
		List<GameObject> pageChildList = launcherController.pageChildList;
		List<App> appList = launcherController.appList;
		drop = AdjustSamePageDropIndex (drop);

//		Log(TAG, "ChildExChange: dragIndex:"+iconMovingInfo.dragIndex+" dropIndex:"+iconMovingInfo.dropIndex);

		GameObject childDrag = pageChildList [drag];
		GameObject childDrop = pageChildList [drop];
		Location dropPos3D = childDrop.GetComponent<IconController> ().originLocation;
		int moveNum = drop - drag;
		int absMoveNum = Mathf.Abs (moveNum);
		int nextIndex = drag;
		Location toLocation = pageChildList [drag].GetComponent<IconController>().originLocation;
		for (int i = 0; i < absMoveNum; i++) {
			nextIndex = moveNum > 0 ? nextIndex + 1 : nextIndex - 1;
			GameObject child = pageChildList [nextIndex];
			IconController childControl = child.GetComponent<IconController> ();
			Location nextLocation = childControl.originLocation;
			childControl.MoveToLocation (toLocation);
//			Log (TAG, "nextPos icon:"+nextIndex+" oriLocation:"+nextLocation+" to Location:"+toLocation);
			toLocation = nextLocation;
		}
		childDrag.GetComponent<IconController> ().SetOriLocation (dropPos3D);

		App app = appList[drag];
		appList.RemoveAt (drag);
		pageChildList.RemoveAt (drag);
		appList.Insert (drop, app);
		pageChildList.Insert (drop, childDrag);
		iconMovingInfo.dragIndex = iconMovingInfo.dropIndex;

//		Log(TAG, "CurrentPageSwap: childDrag:"+childDrag.GetComponent<IconController> ().originLocation+" childDrop:"+childDrop.GetComponent<IconController> ().originLocation);
	}

	void DestoryFirstEmptyOnDropPageAndResetDropInfo(){
		List<App> appList = launcherController.appList;
		List<GameObject> iconList = launcherController.pageChildList;

		GameObject dragIcon = swapInfo.dragIcon;
		IconController iconController = dragIcon.GetComponent<IconController> ();
//		launcherController.Log (TAG, "DestoryFirstEmptyOnDropPageAndResetDropInfo swapInfo.firstEmptyIconLocationOnDropPage:"+swapInfo.firstEmptyIconLocationOnDropPage);
		swapInfo.dropLocation = swapInfo.firstEmptyIconLocationOnDropPage;
		iconMovingInfo.dropIndex = swapInfo.firstEmptyIconIndexOnDropPage;

		appList.RemoveAt (swapInfo.firstEmptyIconIndexOnDropPage);
		iconList.RemoveAt (swapInfo.firstEmptyIconIndexOnDropPage);
		appList.Insert (swapInfo.firstEmptyIconIndexOnDropPage, swapInfo.dragModel);
		iconList.Insert (swapInfo.firstEmptyIconIndexOnDropPage, swapInfo.dragIcon);

		IconController firstPlaceholder = swapInfo.firstEmptyIconIconOnDropPage.GetComponent<IconController> ();
		firstPlaceholder.DestorySelt (true);
	}

	int GetLastIndexOnPageByIndex(int index){
		LauncherModel launcherModel = launcherController.launcherModel;
		int pageItemNum = launcherModel.pageItemNum;
		int pageNum = index/pageItemNum;
		int firstIndexOnPage = pageNum*pageItemNum;
		int lastIndexOnPage = firstIndexOnPage+pageItemNum-1;
		return lastIndexOnPage;
	}

	void ResetSwapInfo(int drag, int drop){
		List<App> appList = launcherController.appList;
		List<GameObject> iconList = launcherController.pageChildList;
		List<GameObject> pageList = launcherController.pageList;
		int pageItemNum = launcherController.launcherModel.pageItemNum;
		swapInfo.dragIndex = drag;
		swapInfo.dragModel = appList [drag];
		swapInfo.dragIcon = iconList [drag];
		swapInfo.dragLocation = swapInfo.dragIcon.GetComponent<IconController> ().originLocation;

		swapInfo.dropIndex = drop;
		swapInfo.dropModel = appList [drop];
		swapInfo.dropIcon = iconList [drop];
		swapInfo.dragPage = drag / pageItemNum;
		swapInfo.dropPage = drop / pageItemNum;
		swapInfo.dropLocation = swapInfo.dropIcon.GetComponent<IconController> ().originLocation;

		swapInfo.lastIndexOnDragPage = GetLastIndexOnPageByIndex (drag);
		swapInfo.lastModelOnDragPage = launcherController.appList[swapInfo.lastIndexOnDragPage];

		swapInfo.lastIndexOnDropPage = GetLastIndexOnPageByIndex(drop);
		swapInfo.lastModelOnDropPage = launcherController.appList[swapInfo.lastIndexOnDropPage];

		swapInfo.lastIconOnDragPage = launcherController.pageChildList[swapInfo.lastIndexOnDragPage];
		swapInfo.lastLocationOnDragPage = swapInfo.lastIconOnDragPage.GetComponent<IconController> ().originLocation;

		swapInfo.lastIconOnDropPage = launcherController.pageChildList[swapInfo.lastIndexOnDropPage];
		swapInfo.lastLocationOnDropPage = swapInfo.lastIconOnDropPage.GetComponent<IconController> ().originLocation;

		ResetPlaceholder ();

	}

	void ResetPlaceholder(){
		List<App> appList = launcherController.appList;
		List<GameObject> iconList = launcherController.pageChildList;
		int[] dragPageBound = launcherController.getPageBoundByIndex (swapInfo.dragPage);
		for (int i = dragPageBound[0]; i <= dragPageBound[1]; i++) {
			App app = appList [i];
			bool isPlaceholder = app.isPlaceholder;
			if (isPlaceholder) {
				int index = i;
				swapInfo.firstEmptyIconIndexOnDragPage = index;
				swapInfo.firstEmptyIconModelOnDragPage = appList [index];
				swapInfo.firstEmptyIconIconOnDragPage = iconList [index];
				swapInfo.firstEmptyIconLocationOnDragPage = iconList [index].GetComponent<IconController> ().originLocation;

				break;
			}
			swapInfo.firstEmptyIconIndexOnDragPage = -1;
		}
//		launcherController.Log (TAG, "drag page firstPlaceholder index:"+swapInfo.firstEmptyIconIndexOnDragPage);

		int[] dropPageBound = launcherController.getPageBoundByIndex (swapInfo.dropPage);
		for (int i = dropPageBound[0]; i <= dropPageBound[1]; i++) {
			App app = appList [i];
			bool isPlaceholder = app.isPlaceholder;
			if (isPlaceholder) {
				int index = i;
				swapInfo.firstEmptyIconIndexOnDropPage = index;
				swapInfo.firstEmptyIconModelOnDropPage = appList [index];
				swapInfo.firstEmptyIconIconOnDropPage = iconList [index];
				swapInfo.firstEmptyIconLocationOnDropPage = iconList [index].GetComponent<IconController> ().originLocation;
				break;
			}
			swapInfo.firstEmptyIconIndexOnDropPage = -1;
		}
//		launcherController.Log (TAG, "drop page firstPlaceholder index:"+swapInfo.firstEmptyIconIndexOnDropPage);
	}

	int AdjustSamePageDropIndex(int dropIndex){
		List<App> appList = launcherController.appList;
		if (appList [dropIndex].isPlaceholder) {
			dropIndex = swapInfo.firstEmptyIconIndexOnDropPage - 1;
			iconMovingInfo.dropIndex = dropIndex;
//			launcherController.Log (TAG, "AdjustTouchIndex drop:"+dropIndex);
		}
		return dropIndex;
	}


}
