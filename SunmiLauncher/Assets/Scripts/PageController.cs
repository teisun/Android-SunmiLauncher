using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Correcting utils. 
/// </summary>
public class CorrectingUtils{
	/// <summary>
	/// 有些android手机底部有navigation bar,手指的触摸点相dui于屏幕高度的，所以需要减去或加上navigation bar的高度来tiao整
	/// </summary>
	/// <param name="eventData">Event data.</param>
	/// <param name="cValue">C value.</param>
	public static void PointerEventCorrecting(ref PointerEventData eventData, float cValue){
		eventData.position = new Vector2 (eventData.position.x, eventData.position.y + cValue);
	}
}

public class TouchInfo{
	public bool pageBeginDrag;
	public bool touchable = false;
	public int pageDragId = -1;
	public int iconDragId = -1;

	//control flip
	public float beginTime;
	public float endTime;
	public float velocityX;
	public Vector2 beginDragPos;
	public Vector2 currentTouchPos;

	//Controls whether to trigger click events
	public bool isInterceptClick = false;//Whether to intercept event,true:container processing;false:sub view processing
	public float pointerDownX;
	public float pointerDownY;

	public bool isInvoking = false;
}

public class PageController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, LongClickListener, IPointerClickHandler  {


	const string TAG = "PageController";
	const float finsihInvokeTime = 0.8f;

	const float moveBalance = 5f;
	const float flipVelocity = 210f;
	const float flipTime = 0.23f;

	private float flipBoundary = 80f;

	[HideInInspector]
	public GameObject currentPage;

	[HideInInspector]
	public int pageSize = 10;

	private LauncherController mLauncherController;
	private IconDragController mIconDragController;

	[HideInInspector]
	public LauncherModel launcherModel;

	private LongClickUtils longClickUtils;
	private GameObject currentSelectedIcon;
	public TouchInfo touchInfo = new TouchInfo ();

	public void SetTouchable(bool touchable){
		this.touchInfo.touchable = touchable;
	}

	// Use this for initialization
	void Start () {
		flipBoundary = Screen.width * 0.1f;
		mLauncherController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<LauncherController>();
		mIconDragController = mLauncherController.iconDragController;
		touchInfo.pageBeginDrag = false;
		touchInfo.currentTouchPos = Vector2.zero;
		longClickUtils = new LongClickUtils ();
		longClickUtils.longClickListener = this;
	}
	
	// Update is called once per frame
	void Update(){
		if (longClickUtils != null) {
			longClickUtils.Update ();
		}

	}

	public int getCurrentPosition(){
//		mLauncherController.Log (TAG, "getCurrentPosition currentPage:"+currentPage);
		PageLayout control = currentPage.GetComponent<PageLayout> ();
//		mLauncherController.Log (TAG, "getCurrentPosition pageNumber:"+control.pageNumber);
		return control.pageNumber;
	}

	public int getLinkagePosition(){
		PageLayout control = currentPage.GetComponent<PageLayout> ();
		return control.getLinkagePosition ();
	}

	public int PointToIndex(Vector2 press){
//		mLauncherController.Log(TAG, "PointToIndex pageNumber:"+getCurrentPosition());
		for (int i = 0; i < currentPage.transform.childCount; i++) {
			IconController iconController = currentPage.transform.GetChild (i).GetComponent<IconController>();
//			mLauncherController.Log(TAG, "iconController.originLocation:"+iconController.originLocation);
			Vector2 leftBottom = new Vector2 (iconController.originLocation.position3D.x, iconController.originLocation.position3D.y);
			Vector2 rightTop = new Vector2 (leftBottom.x+launcherModel.itemWidth, leftBottom.y+launcherModel.itemHeight);
//			mLauncherController.Log(TAG, "launcherModel.itemWidth:"+launcherModel.itemWidth+" launcherModel.itemHeight:"+launcherModel.itemHeight);
			bool inLeftBottom = (press.x >= leftBottom.x&&press.y>=leftBottom.y);
			bool inRightTop = (press.x <= rightTop.x&&press.y<=rightTop.y);
			if (inLeftBottom&&inRightTop) {
				Location location = iconController.originLocation;
				int index = location.index;
				return index;
			}
		}
		return -1;
	}

	void showSelected(PointerEventData eventData){
		
		if (currentSelectedIcon == null) {
			int index = PointToIndex (eventData.position);
//			mLauncherController.Log (TAG, "showSelected index:"+index);
			currentSelectedIcon = mLauncherController.pageChildList [index];
			IconController action = currentSelectedIcon.GetComponent<IconController> ();
//			mLauncherController.Log (TAG, "ShowSelectedBackground");
			action.ShowSelectedBackground ();
		}
	}

	void cancelSelected(){
		if (currentSelectedIcon != null) {
//			mLauncherController.Log (TAG, "DestroySelectedBackground");
			IconController action = currentSelectedIcon.GetComponent<IconController> ();
			action.DestroySelectedBackground ();
//			mLauncherController.Log (TAG, "currentSelectedIcon = null");
			currentSelectedIcon = null;
		}
	}

	/*page slide callback*/
	public void OnPageLeftSlide(float offsetX){
		List<GameObject> pageList = mLauncherController.pageList;
		//左滑 drive next page
		int position = getLinkagePosition();
		if (position < pageSize && position>=0) {
			PageLayout control = pageList [position].GetComponent<PageLayout>();
			IFlipTween tweener = control.flipTween;
			tweener.LeftSlide (offsetX);
		}
	}

	public void OnPageRightSlide(float offsetX){
		List<GameObject> pageList = mLauncherController.pageList;
		//右滑 drive pre page
		int position = getLinkagePosition();
		if (position < pageSize && position>=0) {
			PageLayout control = pageList [position].GetComponent<PageLayout>();
			IFlipTween tweener = control.flipTween;
			tweener.RightSlide (offsetX);
		}
	}

	/*page exit callback*/
	public void OnPageLeftExit(){
		List<GameObject> pageList = mLauncherController.pageList;
		//左滑 drive next page
		int position = getCurrentPosition();

		if (position + 1 < pageSize) {
			PageLayout control = pageList [position + 1].GetComponent<PageLayout> ();
			IFlipTween tweener = control.flipTween;
			tweener.RightEnter ();
			SetTouchable (true);
			currentPage = pageList [position + 1];

		} else if (position == pageSize - 1) {
			PageLayout control = currentPage.GetComponent<PageLayout> ();
			IFlipTween tweener = control.flipTween;
			SetTouchable (true);
			tweener.LeftReset ();
		}
	}

	public void OnPageRightExit(){
		List<GameObject> pageList = mLauncherController.pageList;
		//右滑 drive pre page
		int position = getCurrentPosition();
//		mLauncherController.Log (TAG, "OnPageRightExit position:"+position);
		if (position - 1 >= 0) {
			PageLayout control = pageList [position - 1].GetComponent<PageLayout> ();
			IFlipTween tweener = control.flipTween;
			SetTouchable (true);
			tweener.LeftEnter ();
			currentPage = pageList [position - 1];
		} else if (position == 0) {
			PageLayout control = currentPage.GetComponent<PageLayout> ();
			IFlipTween tweener = control.flipTween;
			SetTouchable (true);
			tweener.RightReset ();
		}
	}

	/*page enter callback*/
	public void OnPageLeftReset(){
		List<GameObject> pageList = mLauncherController.pageList;
		//左滑 drive next page
		int position = getCurrentPosition();
		if (position+1 < pageSize) {
			PageLayout control = pageList [position + 1].GetComponent<PageLayout>();
			IFlipTween tweener = control.flipTween;
			tweener.RightReset ();
		}

	}

	public void OnPageRightReset(){
		List<GameObject> pageList = mLauncherController.pageList;
		//右滑 drive pre page
		int position = getCurrentPosition();
		if (position-1 >= 0) {
			PageLayout control = pageList [position - 1].GetComponent<PageLayout>();
			IFlipTween tweener = control.flipTween;
			//			Log (TAG, "OnPageRightReset call tweener.LeftReset()");
			tweener.LeftReset ();
		}
	}


	//listener callback

	#region IPointerDownHandler implementation
	public void OnPointerDown (PointerEventData eventData)
	{
		mLauncherController.Log (TAG, "OnPointerDown");
		CorrectingUtils.PointerEventCorrecting (ref eventData, -launcherModel.navigationBarHeight);
		if (!touchInfo.touchable) {
			mLauncherController.Log (TAG, "OnPointerDown !touchInfo.touchable");
			return;
		}

		int index = PointToIndex (eventData.position);
		mLauncherController.Log (TAG, "OnPointerDown PointToIndex index:"+index);
		App app = mLauncherController.appList [index];
		mLauncherController.Log (TAG, "OnPointerDown app:"+app.appName);
		if (!mIconDragController.IsIconDragMode()) {
			longClickUtils.OnPointerDown (eventData);
			touchInfo.pointerDownX = eventData.position.x;
			touchInfo.pointerDownY = eventData.position.y;
			if (!app.isPlaceholder) {
				mLauncherController.Log (TAG, "OnPointerDown showSelected");
				showSelected (eventData);
			}
		}

		if (!touchInfo.pageBeginDrag) {
			//			Debug.Log ("OnBeginDrag time:"+Time.time);
			mLauncherController.Log(TAG, "OnPointerDown:if (!touchInfo.pageBeginDrag) ");
			touchInfo.beginTime = Time.time;
			touchInfo.currentTouchPos = eventData.position;
			touchInfo.beginDragPos = eventData.position;
			touchInfo.pageBeginDrag = true;
			touchInfo.pageDragId = eventData.pointerId;
		}

	}
	#endregion

	#region LongClickListener implementation

	public void OnLongClick (PointerEventData eventData)
	{
		CorrectingUtils.PointerEventCorrecting (ref eventData, -launcherModel.navigationBarHeight);
		int index = PointToIndex (eventData.position);
		App app = mLauncherController.appList [index];
		if (app.isPlaceholder) {
			return;
		}
		cancelPageDrag ();
		mLauncherController.Log (TAG, "OnLongClick eventData.position:"+eventData.position);
		mIconDragController.OnIconLongClick (eventData, currentPage);
		touchInfo.iconDragId = eventData.pointerId;
		touchInfo.isInterceptClick = true;
		mLauncherController.CreateNewPageAndFill ();//create a new page
//		mLauncherController.Log (TAG, "OnLongClick  pageSize:"+mLauncherController.pageController.pageSize+" appList size:"+mLauncherController.appList.Count+" pageList size:"+mLauncherController.pageList.Count);
	}

	#endregion

	void OnIconRelease(){
		mLauncherController.Log (TAG, "OnIconRelease");
		mLauncherController.ClearEmptyPage ();
		mLauncherController.ClearRegress ();
		mLauncherController.SaveAppList ();
		cancelPageDrag ();
	}

	#region IPointerUpHandler implementation
	public void OnPointerUp (PointerEventData eventData)
	{
		CorrectingUtils.PointerEventCorrecting (ref eventData, -launcherModel.navigationBarHeight);
		if (!touchInfo.touchable) {
			return;
		}

		mLauncherController.Log (TAG, "OnPointerUp eventData.position:"+eventData.position);
		cancelSelected ();
		longClickUtils.OnPointerUp (eventData);
//		mLauncherController.Log (TAG, "OnPointerUp eventData.pointerId:"+eventData.pointerId+" touchInfo.pageDragId:"+touchInfo.pageDragId);
		if (mIconDragController.IsIconDragMode () && eventData.pointerId == touchInfo.iconDragId) {
			mIconDragController.OnIconPointerUp (eventData, gameObject);
			Invoke ("OnIconRelease", LauncherController.ANIM_DURATION);
		} else if(eventData.pointerId == touchInfo.pageDragId){
			mLauncherController.Log (TAG, "OnPointerUp touchInfo.isInterceptClick:"+touchInfo.isInterceptClick);
//			if (!touchInfo.isInterceptClick) {
//				return;
//			}
			float disX = Mathf.Abs(touchInfo.pointerDownX - eventData.position.x);
			float disY = Mathf.Abs(touchInfo.pointerDownY - eventData.position.y);
			//		Debug.Log ("disX:"+disX+" disY:"+disY);
			mLauncherController.Log (TAG, "disX:"+disX+" disY:"+disY);
			if (disX >= moveBalance || disY >= moveBalance) {
				touchInfo.isInterceptClick = true;
			} else {
				touchInfo.isInterceptClick = false;
			}

			if (!touchInfo.isInterceptClick) {
				return;
			}

			DragPage (eventData);
			cancelPageDrag ();
			EndSlide (eventData);
		}

	}
	#endregion

	void cancelPageDrag(){
		touchInfo.pageBeginDrag = false;
		touchInfo.pageDragId = -1;
		touchInfo.currentTouchPos = Vector2.zero;
	}

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData)
	{
//		CorrectingUtils.PointerEventCorrecting (ref eventData, -launcherModel.navigationBarHeight);
		if (!touchInfo.touchable) {
			return;
		}

		mLauncherController.Log (TAG, "OnPointerClick eventData.position:"+eventData.position);
		if (mIconDragController.IsIconDragMode()) {
			return;
		}

		if (touchInfo.isInterceptClick) {
			mLauncherController.Log (TAG, "OnPointerClick isIntercept:"+touchInfo.isInterceptClick);
			touchInfo.isInterceptClick = false;
			return;
		}
		//click 
		int index = PointToIndex(eventData.position);
		App app = mLauncherController.appList [index];
		if (!app.isPlaceholder) {
			mLauncherController.OpenApp(app);
		}
		touchInfo.isInvoking = false;
		touchInfo.isInterceptClick = false;
	}
	#endregion	


	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		CorrectingUtils.PointerEventCorrecting (ref eventData, -launcherModel.navigationBarHeight);
		if (!touchInfo.touchable) {
			mLauncherController.Log (TAG, "!touchInfo.touchable");
			return;
		}

		//		Debug.Log ("OnPointerDown:"+eventData.position);
		mLauncherController.Log (TAG, "OnBeginDrag eventData.position:"+eventData.position);
		if (mIconDragController.IsIconDragMode ()&&eventData.pointerId == touchInfo.iconDragId) {
//			mLauncherController.Log (TAG, "1 eventData.pointerId:"+eventData.pointerId);
			mIconDragController.OnIconBeginDrag (eventData, currentPage);
			//			return;
		}else{
//			mLauncherController.Log (TAG, "2 eventData.pointerId:"+eventData.pointerId);
			float disX = Mathf.Abs(touchInfo.pointerDownX - eventData.position.x);
			float disY = Mathf.Abs(touchInfo.pointerDownY - eventData.position.y);
			//		Debug.Log ("disX:"+disX+" disY:"+disY);
			mLauncherController.Log (TAG, "disX:"+disX+" disY:"+disY);
			if (disX >= moveBalance || disY >= moveBalance) {
				touchInfo.isInterceptClick = true;
				longClickUtils.cancel ();
				DragPage (eventData);
			} else {
				touchInfo.isInterceptClick = false;
			}


		}
	}
	#endregion	

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		CorrectingUtils.PointerEventCorrecting (ref eventData, -launcherModel.navigationBarHeight);
		if (!touchInfo.touchable) {
			return;
		}
//		mLauncherController.Log (TAG, "OnDrag eventData.position:"+eventData.position);
		if (mIconDragController.IsIconDragMode ()&&eventData.pointerId == touchInfo.iconDragId) {
//			mLauncherController.Log (TAG, "1 eventData.pointerId:"+eventData.pointerId);
			DragIcon(eventData);

		}else if (eventData.pointerId == touchInfo.pageDragId) {
//			mLauncherController.Log (TAG, "2 eventData.pointerId:"+eventData.pointerId);
			if (!touchInfo.isInterceptClick) {
				return;
			}
			DragPage (eventData);
		}
	}

	void DragIcon(PointerEventData eventData){
		mIconDragController.OnIconDrag (eventData, currentPage);
		bool boundaryRight = Screen.width - eventData.position.x < flipBoundary;
		bool boundaryLeft = eventData.position.x < flipBoundary;
		if (boundaryRight) {
			if(getCurrentPosition () < pageSize-1){
				mLauncherController.Log (TAG, "Invoke LeftFlipCurrentPage");
				Invoke ("LeftFlipCurrentPage", 0.5f);
			}
		} else if (boundaryLeft) {
			if (getCurrentPosition () != 0) {
				mLauncherController.Log (TAG, "Invoke RightFlipCurrentPage");
				Invoke ("RightFlipCurrentPage", 0.5f);
			}
		} else {
			mLauncherController.Log (TAG, "CancelInvoke ()");
			CancelInvoke ("LeftFlipCurrentPage");
			CancelInvoke ("RightFlipCurrentPage");
		}
	}

	void DragPage(PointerEventData eventData){
		float offsetX =  eventData.position.x - touchInfo.currentTouchPos.x;
		touchInfo.currentTouchPos = eventData.position;
		PageLayout pageLayout = currentPage.GetComponent<PageLayout> ();
		if (offsetX < 0) {
			//左滑 drive next page
			mLauncherController.Log (TAG, "OnDrag: 左滑"+"pageLayout.flipTween.LeftSlide offsetX:"+offsetX);
			pageLayout.flipTween.LeftSlide (offsetX); 
			OnPageLeftSlide (offsetX);

		} else if (offsetX > 0) {
			//右滑 drive pre page
			mLauncherController.Log (TAG, "OnDrag: 右滑"+"pageLayout.flipTween.LeftSlide offsetX:"+offsetX);
			pageLayout.flipTween.RightSlide (offsetX);
			OnPageRightSlide (offsetX);
		}
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		CorrectingUtils.PointerEventCorrecting (ref eventData, -launcherModel.navigationBarHeight);
		if (!touchInfo.touchable) {
			return;
		}

		//		Debug.Log ("OnEndDrag");
		mLauncherController.Log (TAG, "OnEndDrag eventData.position:"+eventData.position);
		if (mIconDragController.IsIconDragMode ()&&eventData.pointerId == touchInfo.iconDragId) {
			mIconDragController.OnIconEndDrag (eventData, currentPage);
//			mLauncherController.Log (TAG, "1 eventData.pointerId:"+eventData.pointerId);
			touchInfo.iconDragId = -1;
			//			return;
		}else if (eventData.pointerId == touchInfo.pageDragId) {
//			mLauncherController.Log (TAG, "2 eventData.pointerId:"+eventData.pointerId);
//			touchInfo.pageBeginDrag = false;
//			touchInfo.currentTouchPos = Vector2.zero;
//			EndSlide (eventData);

		}
	}

	#endregion

	//----------- class method ------------

	void EndSlide(PointerEventData eventData){
		float distanceX = touchInfo.beginDragPos.x - eventData.position.x;
		touchInfo.endTime = Time.time;
		float deltaTime = touchInfo.endTime - touchInfo.beginTime;
		touchInfo.velocityX = distanceX / deltaTime;
//					mLauncherController.Log (TAG, "OnEndDrag currentTouchPos:"+touchInfo.currentTouchPos+" distanceX:"+distanceX+" endTime:"+touchInfo.endTime+" beginTime:"+touchInfo.beginTime+" velocity:"+touchInfo.velocity);
		//			Debug.Log ("velocity :"+ velocity);
		PageLayout pageLayout = currentPage.GetComponent<PageLayout> ();
		mLauncherController.Log (TAG, "EndSlide velocity="+touchInfo.velocityX);
		mLauncherController.Log (TAG, "EndSlide distanceX="+distanceX);
		mLauncherController.Log (TAG, "EndSlide deltaTime="+deltaTime);
		if (touchInfo.velocityX > flipVelocity && deltaTime < flipTime) {
			//左滑
			mLauncherController.Log (TAG, "EndSlide velocity > filpVelocity ");
			LeftFlip (pageLayout);

		} else if (touchInfo.velocityX < -flipVelocity && deltaTime < flipTime ) {
			//右滑
			mLauncherController.Log (TAG, "EndSlide velocity < -filpVelocity ");
			RightFlip (pageLayout);
		}
		else{
			FlipByDistanceX (distanceX, pageLayout);
		}
	}

	void LeftFlip(PageLayout pageLayout){
		//左滑
		touchInfo.touchable = false;
		pageLayout.flipTween.LeftExit ();
		OnPageLeftExit ();
	}

	void LeftReset(PageLayout pageLayout){
		pageLayout.flipTween.LeftReset ();
		OnPageLeftReset ();
	}

	void RightFlip(PageLayout pageLayout){
		touchInfo.touchable = false;
		pageLayout.flipTween.RightExit ();
		mLauncherController.Log (TAG, "RightFlip");
		OnPageRightExit ();
	}

	void RightReset(PageLayout pageLayout){
		pageLayout.flipTween.RightReset ();
		OnPageRightReset ();
	}

	void FlipByDistanceX(float distanceX, PageLayout pageLayout){
		if (distanceX > 0) {
			mLauncherController.Log (TAG, "FlipByDistanceX distanceX > 0 ");
			//左滑 drive next page
			if (distanceX >= (Screen.width / 2)) {
				LeftFlip (pageLayout);
			} else {
				//						mLauncherController.currentPage = gameObject;
				//mLauncherController.Log (TAG, "OnEndDrag call tweener.LeftReset() position:"+position);
				LeftReset (pageLayout);
			}
		} else {
			mLauncherController.Log (TAG, "FlipByDistanceX distanceX < 0 ");
			//右滑 drive pre page
			float absDistanceX = Mathf.Abs(distanceX);
			if (absDistanceX >= (Screen.width / 2)) {
				RightFlip (pageLayout);
			} else {
				//						mLauncherController.currentPage = gameObject;
				RightReset(pageLayout);
			}
		}
	}

	void LeftFlipCurrentPage(){
		if (touchInfo.isInvoking) {
			return;
		}
		mLauncherController.Log (TAG, "LeftFlipCurrentPage");
		touchInfo.isInvoking = true;
		PageLayout pageLayout = currentPage.GetComponent<PageLayout> ();
		LeftFlip (pageLayout);
		Invoke ("FinishInvoke", finsihInvokeTime);
	}


	void RightFlipCurrentPage(){
		if (touchInfo.isInvoking) {
			return;
		}
		mLauncherController.Log (TAG, "RightFlipCurrentPage");
		touchInfo.isInvoking = true;
		PageLayout pageLayout = currentPage.GetComponent<PageLayout> ();
		RightFlip (pageLayout);
		Invoke ("FinishInvoke", finsihInvokeTime);
	}


	void FinishInvoke(){
		mLauncherController.Log (TAG, "FinishInvoke");
		touchInfo.isInvoking = false;
	}

	public void PageEntry(int pageIndex){
		GameObject page = mLauncherController.pageList [pageIndex];
		PageLayout pageLayout = page.GetComponent<PageLayout> ();
		pageLayout.Entry ();
		currentPage = page;
	}

}
