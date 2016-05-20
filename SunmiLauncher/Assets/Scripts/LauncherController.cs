using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using System.IO;
using DG.Tweening;
using UnityEngine.EventSystems;

public class GameUtils{
	public static GameObject GetChildByName(GameObject parent, string name){
		foreach (Transform t in parent.GetComponentsInChildren<Transform>()) {
			if (t.name == name) {
				return t.gameObject;
			}
		}
		return null;
	}
}

public class LauncherController : MonoBehaviour {

	public static float ICON_RISING_DISTANCE = -Screen.height * 0.09f;
	const string TAG = "LauncherController";

	//test
	public Text tempText;
	public Image icon;

	public const float ANIM_DURATION = 0.3f;

	public static int MODE = 0;

	[HideInInspector]
	public List<App> appList;

	public GameObject pageContainer;

	[HideInInspector]
	public PageController pageController;
	[HideInInspector]
	public IconDragController iconDragController;

	public AndroidJavaObject launcherPlugin;

	public GameObject launcherCanvas;

	[HideInInspector]
	public LauncherModel launcherModel;

	public GameObject pageLayoutPrefab;

	public GameObject pageChildPrefab;

	[HideInInspector]
	public List<GameObject> pageList = new List<GameObject>();

	[HideInInspector]
	public List<GameObject> pageChildList = new List<GameObject> ();//管理icon排序的list

	public GameObject uninstallDialog;

	public GameObject statusBarBg;

	public GameObject trashRange;
	GameObject trash;

	void Awake(){
		Screen.fullScreen = false;
		CanvasScaler canvasScaler = launcherCanvas.GetComponent<CanvasScaler> ();
		canvasScaler.referenceResolution = new Vector2 (Screen.width, Screen.height-72);
		pageController = pageContainer.GetComponent<PageController> ();
		iconDragController = pageContainer.GetComponent<IconDragController> ();
		trashRange = GameObject.FindGameObjectWithTag ("TrashRange");
		trash = trashRange.transform.GetChild (0).gameObject;
		uninstallDialog = GameObject.FindGameObjectWithTag ("UninstallDialog");
		uninstallDialog.transform.localScale = Vector3.zero;

		AndroidJavaClass jc=new AndroidJavaClass("com.unity3d.player.UnityPlayer");  
		launcherPlugin = jc.GetStatic<AndroidJavaObject> ("currentActivity");
		ShowStatusBar ();
		GetLauncherModel ();
//		mock();
//		Log("LauncherController", "Awake()");
	}

	//------------call android plugin method------------//

	public void OpenApp(App app){
		string json = JsonWriter.Serialize (app);
		string[] paramsArr = new string[1];
		paramsArr[0] = json;
		launcherPlugin.Call ("openApp", paramsArr);
	}

	public void SaveAppList(){
		string jsonList = JsonWriter.Serialize (appList);
//		Log (TAG, "SaveAppList jsonList:"+jsonList);
		string[] paramsArr = new string[1];
		paramsArr[0] = jsonList;
		launcherPlugin.Call ("saveAppList", paramsArr);
	}

	public void Log(string tag, string msg){
		Debug.Log (msg);
		string[] paramsArr = new string[2];
		paramsArr[0] = tag;
		paramsArr[1] = msg;
		launcherPlugin.Call ("debug", paramsArr);
	}

	public void GetLauncherModel(){
		string[] paramsArr = new string[2];
		paramsArr[0] = "LauncherController";
		paramsArr[1] = "MessageLauncherModel";
		launcherPlugin.Call ("getLauncherModel", paramsArr);
	}


	public void ShowStatusBar(){
		launcherPlugin.Call ("showStatusBar");
	}

	public void GetAppList(){
		string[] paramsArr = new string[2];
		paramsArr[0] = "LauncherController";
		paramsArr[1] = "MessageAppList";
		launcherPlugin.Call ("getAppList", paramsArr);

//		string json = "[{\"appName\":\"钉钉\",\"packageName\":\"com.alibaba.android.rimet\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.alibaba.android.rimet.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"PDF阅读器\",\"packageName\":\"com.upstudio.fastviewer.pdf\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.upstudio.fastviewer.pdf.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"微信\",\"packageName\":\"com.tencent.mm\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.tencent.mm.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"天天看\",\"packageName\":\"com.tiantiankan.ttkvod\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.tiantiankan.ttkvod.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"test\",\"packageName\":\"com.market\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.market.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"春秋航空\",\"packageName\":\"com.china3s.android\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.china3s.android.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"影梭\",\"packageName\":\"com.github.shadowsocks\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.github.shadowsocks.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"美团外卖\",\"packageName\":\"com.sankuai.meituan.takeoutnew\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.sankuai.meituan.takeoutnew.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"example\",\"packageName\":\"com.example\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.example.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"nubia电工\",\"packageName\":\"cn.nubia.powermanage\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/cn.nubia.powermanage.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"得到\",\"packageName\":\"com.luojilab.player\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.luojilab.player.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"tomcat-LineWrapLayout\",\"packageName\":\"tomcat.linewraplayout\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/tomcat.linewraplayout.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"锤子桌面\",\"packageName\":\"com.smartisanos.home\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.smartisanos.home.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"暴风魔镜\",\"packageName\":\"com.baofeng.mj\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.baofeng.mj.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"Teambition\",\"packageName\":\"com.teambition.teambition\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.teambition.teambition.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"最美手电筒\",\"packageName\":\"com.nanshan.torch\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.nanshan.torch.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"Unity Remote 4\",\"packageName\":\"com.unity3d.genericremote\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.unity3d.genericremote.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"云家政\",\"packageName\":\"com.yjz\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.yjz.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0},{\"appName\":\"猎豹浏览器\",\"packageName\":\"com.ijinshan.browser_fast\",\"iconUrl\":\"/storage/sdcard0/launcher_icon/com.ijinshan.browser_fast.png\",\"beUsed\":false,\"isPlaceholder\":false,\"appStatus\":0,\"progress\":0,\"pushNum\":0}]";
//		messageAppList (json);
	}


	//------------android call back------------------//

	void mock(){
		launcherModel = new LauncherModel ();
		launcherModel.pageItemNum = 6;
		launcherModel.columnNum = 2;
		launcherModel.currentPage = 0;
		launcherModel.rowNum = 3;
		launcherModel.screenWidth = Screen.width;
		launcherModel.screenHeight = Screen.height;
		launcherModel.itemWidth = launcherModel.screenWidth / launcherModel.columnNum;
		launcherModel.itemHeight = launcherModel.screenHeight / launcherModel.rowNum;
		launcherModel.statusBarHeight = 75;

		RectTransform rt = statusBarBg.GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2 (launcherModel.screenWidth, launcherModel.statusBarHeight);

		appList = new List<App> ();
		for (int i = 0; i < 58; i++) {
			App app = new App ();
//			app.iconUrl =
			appList.Add (app);
		}
		initLauncher();
	}

	public void MessageUninstalled(string appJson){
		Log (TAG, "MessageUninstalled appJson:"+appJson);
		App app = JsonReader.Deserialize<App> (appJson);
		DialogController dialogController = uninstallDialog.GetComponent<DialogController>();
		dialogController.uninstalled ();
		OnUninstalled(app);
	}

	public void MessageInstalled(string appListJson){
		Log (TAG, "MessageInstalled appListJson:"+appListJson);
		List<App> newAppList = JsonReader.Deserialize<List<App>> (appListJson);
//		Log (TAG, "MessageInstalled list size:"+appList.Count);
		foreach(App app in newAppList){
			if (!appList.Contains (app)) {
				AddNewApp(app);
			}
		}
		SaveAppList ();
	}

	void MessageLauncherModel(string json){
		tempText.text = json;
		launcherModel = JsonReader.Deserialize<LauncherModel> (json);
		pageController.launcherModel = launcherModel;
		RectTransform rt = statusBarBg.GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2 (launcherModel.screenWidth, launcherModel.statusBarHeight);
		GetAppList();
	}

	void MessageAppList(string json){
		tempText.text = json;
		appList = JsonReader.Deserialize<List<App>> (json);

		//test
//		App app1 = new App();
//		app1.SetIsPlaceholder (true);
//		appList.Insert(2, app1);
//		App app2 = new App();
//		app2.SetIsPlaceholder (true);
//		appList.Insert(3, app2);
//		App app3 = new App();
//		app3.SetIsPlaceholder (true);
//		appList.Insert(4, app3);
//		App app4 = new App();
//		app4.SetIsPlaceholder (true);
//		appList.Insert(5, app4);

//		Debug.Log ("list size:"+appList.Count);

		int pageItemNum = launcherModel.pageItemNum;
		if (appList.Count / pageItemNum == 0) {
			pageController.pageSize = 1;
		} else if (appList.Count % pageItemNum == 0) {
			pageController.pageSize = appList.Count / pageItemNum;
		}else{
			pageController.pageSize = appList.Count / pageItemNum + 1;
		}
		tempText.text = appList.Count + "个app";
		initLauncher();
	}

	/*---------------------------*/

	//-----------init page layout and load app icon------------//
	void initTrash(){
		RectTransform rtTrashRange = trashRange.GetComponent<RectTransform> ();
		RectTransform rtTrash = trash.GetComponent<RectTransform> ();
		Vector2 sizeRtTrash = new Vector2 (Screen.width*0.185f, Screen.height*0.104f);
		Vector2 sizeRtTrashRange = new Vector2 (sizeRtTrash.x*1.2f, sizeRtTrash.y*2f);
		rtTrash.sizeDelta = sizeRtTrash;
		rtTrashRange.sizeDelta = sizeRtTrashRange;
		rtTrashRange.anchoredPosition = new Vector2 (0f, -rtTrashRange.sizeDelta.y);
		trashRange.SetActive (false);

	}

	public void initLauncher(){
		initTrash ();
		DialogController control = uninstallDialog.GetComponent<DialogController> ();
		control.initUninstallDialog ();

		RectTransform rt = pageContainer.GetComponent<RectTransform> ();
		launcherModel.navigationBarHeight = Screen.height - launcherModel.statusBarHeight - launcherModel.screenHeight;
		rt.anchoredPosition3D = new Vector3 (0f, launcherModel.navigationBarHeight, 0f);
		rt.sizeDelta = new Vector2 (launcherModel.screenWidth, launcherModel.screenHeight);
		for (int i = 0; i < pageController.pageSize; i++) {
			CreateNewPage (i);
		}
		pageController.currentPage = pageList [0];
		pageController.SetTouchable (true);
		LoadAppIcon (0, appList.Count);
	}

	void LoadAppIcon(int beginIndex, int endIndex){
		int left = 0;
		int bottom = 0;
		int curPage = 0;
		int curRow = 0;
		int pageItemNum = launcherModel.pageItemNum;
		int columnNum = launcherModel.columnNum;
		int itemWidth = launcherModel.itemWidth;
		int itemHeight = launcherModel.itemHeight;
		int pageHeight = launcherModel.screenHeight;
		GameObject pageLayout = null;
		int appNum = endIndex;

		for(int i=beginIndex;i<appNum;i++){
			curPage = i / pageItemNum;
			curRow = i % pageItemNum / columnNum;
			if (i % columnNum == 0) {
				left = 0;
			} else {
				left = itemWidth;
			}
			bottom = curRow * itemHeight;
			pageLayout = pageList [curPage];
			GameObject pageChild = Instantiate (pageChildPrefab);
			IconController iconControl = pageChild.GetComponent<IconController> ();

			App app = appList [i];
			iconControl.setApp (app);
			Vector3 pos3D = new Vector3(left, pageHeight-bottom-launcherModel.itemHeight, 0f);

			PageLayout pageControl = pageLayout.GetComponent<PageLayout> ();
			//			Debug.Log ("LoadAppIcon left="+left+" top="+bottom);
			iconControl.originLocation = new Location (pageControl.pageNumber, pos3D, i);
			pageControl.addChild (pageChild, pos3D, new Vector2(itemWidth, itemHeight));
			pageChildList.Add (pageChild);
		}
	
	}


	public Sprite LoadImgByIO(App app){
		//create file stream
		FileStream fileStream = new FileStream(app.iconUrl, FileMode.Open, FileAccess.Read);
		fileStream.Seek (0, SeekOrigin.Begin);
		//create buffer
		byte[] bytes = new byte[fileStream.Length];
		//read by buffer
		fileStream.Read(bytes, 0, (int)fileStream.Length);
		//release stream
		fileStream.Close();
		fileStream.Dispose ();
		fileStream = null;

		//create Texture
		int width = launcherModel.iconWidth;
		int height = launcherModel.iconWidth;
		Texture2D texture = new Texture2D (width, height);
		texture.LoadImage (bytes);

		//create Sprite
		Rect rect = new Rect(0, 0, texture.width, texture.height);
		Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
//		icon.sprite = sprite;
		return sprite;
	}

	public void addPlaceholder(int pageNum, int index, Location location){
		App app = new App ();
		app.SetIsPlaceholder (true);
		appList.Insert (index, app);

		GameObject pageLayout = pageList [pageNum];
		GameObject pageChild = Instantiate (pageChildPrefab);
		IconController iconControl = pageChild.GetComponent<IconController> ();
		iconControl.setApp (app);
		PageLayout pageControl = pageLayout.GetComponent<PageLayout> ();
		iconControl.originLocation = location;
		pageControl.addChild (pageChild, iconControl.originLocation.position3D, new Vector2 (launcherModel.itemWidth, launcherModel.itemHeight));
		pageChildList.Insert (index, pageChild);
	}

	/// <summary>
	/// Adds the new app.将app加到最后page的第一个空位 或者新建新page放到page的第一个index
	/// </summary>
	/// <param name="app">App.</param>
	public void AddNewApp(App newApp){
		int lastPageNum = pageController.pageSize-1;
		int[] bound = getPageBoundByIndex (lastPageNum);
		int firstPlaceholderIndexOnLastPage = -1;
		for (int i = bound [0]; i <= bound [1]; i++) {
			App app = appList [i];
			if (app.isPlaceholder) {
				firstPlaceholderIndexOnLastPage = i;
				Log (TAG, "AddNewApp firstPlaceholderIndexOnLastPage:"+firstPlaceholderIndexOnLastPage);
				break;
			}
		}
		if (firstPlaceholderIndexOnLastPage == -1) {
			//create new page
			CreateNewPageAndFill ();
			ChangeAppByIndex (bound[1]+1, newApp);
		} else {
			Log (TAG, "AddNewApp firstPlaceholderIndexOnLastPage:"+firstPlaceholderIndexOnLastPage+" new app:"+newApp);
			ChangeAppByIndex (firstPlaceholderIndexOnLastPage, newApp);
		}


	}

	/// <summary>
	/// Changes the index of the app by.
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="app">App.</param>
	void ChangeAppByIndex(int index, App app){
		appList.RemoveAt (index);
		appList.Insert (index, app);
		GameObject icon = pageChildList [index];
		IconController iconController = icon.GetComponent<IconController> ();
		iconController.setApp (app);
	}

	//icon 前移
	public void IconAdvance(int toIndex, int beginIndex, int endIndex, bool anim){
		List<GameObject> iconList = pageChildList;
		Location toLocation = iconList [toIndex].GetComponent<IconController> ().originLocation;
		for (int i = beginIndex; i <= endIndex; i++) {
			GameObject icon = iconList [i];
			IconController iconController = icon.GetComponent<IconController> ();
			Location iconLocation = iconController.originLocation;
			if (iconController.regressLocation == null) {
				iconController.MoveToLocation (toLocation);
			} else {
				iconController.LocationRegress ();
			}
			toLocation = iconLocation;
		}
	}


	//icon 后移,遇到page末尾是空位就干掉空位并停止后移
	public void IconBackward(int beginIndex, int endIndex, bool anim){
		
		List<GameObject> iconList = pageChildList;
		Log (TAG, "IconBackward beginIndex:"+beginIndex+" endIndex:"+endIndex+" iconList size:"+iconList.Count);
		for (int i = beginIndex; i <= endIndex; i++) {
			GameObject icon = iconList [i];
			IconController iconController = icon.GetComponent<IconController> ();
			App app = appList [i];
			if (app.isPlaceholder && i % launcherModel.pageItemNum==(launcherModel.pageItemNum-1)) {
				//page end
				iconList.RemoveAt (i);
				appList.RemoveAt (i);
				iconController.DestorySelt(anim);
				return;
			}
			if (i + 1 < iconList.Count) {
				GameObject nextIcon = iconList [i + 1];
				Location toLocation = nextIcon.GetComponent<IconController> ().originLocation;
				Log (TAG, "IconBackward index:"+i+" originLocation:"+iconController.originLocation+" toLocation:"+toLocation);
				iconController.MoveToLocation (toLocation);

			} else {
				//create a new page 
				Log (TAG, "IconBackward create a new page");
				CreateNewPageAndFill ();

				GameObject nextIcon = iconList [i + 1];
				Location toLocation = nextIcon.GetComponent<IconController> ().originLocation;
				iconController.MoveToLocation (toLocation);
				DestoryApp (appList.Count-launcherModel.pageItemNum, false);
			}

		}
	}

	/// <summary>
	/// Clears the empty page.
	/// </summary>
	public void ClearEmptyPage(){
		Log (TAG, "ClearEmptyPage");
		List<GameObject> iconList = pageChildList;
		foreach (GameObject icon in iconList) {
			IconController iconController = icon.GetComponent<IconController> ();
//			Log (TAG, "app:"+iconController.app.appName+" originLocation:"+iconController.originLocation);
		}
		List<GameObject> removes = new List<GameObject> ();
		for (int i = 0; i < pageList.Count; i++) {
			GameObject page = pageList [i];
			bool isEmpty = true;
			int[] bound = getPageBoundByIndex (i);
			for (int j = bound [0]; j <= bound [1]; j++) {
				App app = appList [j];
				if (!app.isPlaceholder) {
					isEmpty = false;
					break;
				}
			}
			Log (TAG, "page num:" + i + " isEmpty:" + isEmpty);
			if (isEmpty) {
				removes.Add (page);
			}
		}
		bool destoryCurrentPage = false;
		int recordIndex = -1;
		foreach (GameObject page in removes) {
			PageLayout pageLayout = page.GetComponent<PageLayout> ();
			int pageNum = pageLayout.pageNumber;
			if (pageNum == pageController.getCurrentPosition ()) {
				destoryCurrentPage = true;
				recordIndex = pageNum;
			}
			DestoryPage (page);
		}
		if (destoryCurrentPage) {
			pageController.PageEntry (recordIndex-1);
		}

	}

	public void DestoryPage(GameObject page){
		Log (TAG, "DestoryPage pre :");
		--pageController.pageSize;
		PageLayout pageLayout = page.GetComponent<PageLayout> ();
		int pageIndex = pageLayout.pageNumber;
		appList.RemoveRange (pageIndex*launcherModel.pageItemNum, launcherModel.pageItemNum);
		pageChildList.RemoveRange (pageIndex*launcherModel.pageItemNum, launcherModel.pageItemNum);
		pageList.Remove (page);
		Destroy (page);
		Log (TAG, "call ResetPageIconInfo ()");
		ResetPageIconInfo ();
	}


	public void ResetPageIconInfo(){
		Log (TAG, "ResetPageIconInfo pre");
		for (int i = 0; i < pageList.Count; i++) {
			GameObject page = pageList [i];
			page.GetComponent<PageLayout> ().pageNumber = i;
			int[] bound = getPageBoundByIndex (i);
			for (int j=bound[0];j<=bound[1];j++) {
				GameObject icon = pageChildList [j];
				IconController iconController = icon.GetComponent<IconController> ();
				iconController.originLocation.pageNum = i;
				int pageItemNum = launcherModel.pageItemNum;
				int index = i * pageItemNum + j%pageItemNum;
//				Log (TAG, "ResetPageIconInfo for2 index:"+index);
				iconController.originLocation.index = index;
				iconController.app =  appList[index];
			}
		}
		Log (TAG, "ResetPageIconInfo after");
	}

	public void CreateNewPageAndFill(){
		CreateNewPage(pageController.pageSize);
		if(iconDragController.currentDragIcon!=null)
			iconDragController.currentDragIcon.transform.SetAsLastSibling();
		pageController.pageSize = pageController.pageSize + 1;
		CreateEnptyApp (launcherModel.pageItemNum);
	}

	public void CreateEnptyApp (int num){
		for (int i = 0; i < num; i++) {
			App app = new App ();
			app.SetIsPlaceholder (true);
			appList.Add (app);
		}
		LoadAppIcon (appList.Count-num, appList.Count);
	}

	public void CreateNewPage(int pageNum){
		GameObject pageLayout = Instantiate (pageLayoutPrefab);
		pageList.Add (pageLayout);
		pageLayout.transform.SetParent (pageContainer.transform);
		pageLayout.transform.localScale = Vector3.one;
		Vector3 position3D = new Vector3 (launcherModel.screenWidth, 0f, 0f);
		if (pageNum == 0) {
			position3D = new Vector3 (0f, 0f, 0f);
		}
		PageLayout control =  pageLayout.GetComponent<PageLayout> ();
		control.pageNumber = pageNum;
		control.setUITransform (position3D);
	}

	/**
	 * get page bound
	 * @param position
	 * @return
	 */
	public int[] getPageBoundByIndex(int pageNum){
		int pageItemNum = launcherModel.pageItemNum;
		int firstPositionOnPage = pageNum*pageItemNum;
		int endPositeionOnPage = firstPositionOnPage + pageItemNum - 1;
		int[] bound = new int[]{firstPositionOnPage, endPositeionOnPage};
		return bound;
	}

	public void DestoryApp(int index, bool anim){
		appList.RemoveAt (index);
		IconController iconController = pageChildList [index].GetComponent<IconController> ();
		pageChildList.RemoveAt (index);
		iconController.DestorySelt (anim);
	}

	public void DestoryIcon(int index){
		App app = appList [index];
		app.SetIsPlaceholder (true);
		IconController iconController = pageChildList [index].GetComponent<IconController> ();
		iconController.DestoryIcon ();
	}

	public void OnUninstalled(App uninstallApp){
		int index = appList.IndexOf (uninstallApp);
		GameObject icon = pageChildList [index];
		IconController iconController = icon.GetComponent<IconController> ();
		//TODO 1.判断是否是page最后一个app，是就置成空位 不是就使page 的app向前移一位后再最后插入一个空位
		int pageNum = index/launcherModel.pageItemNum;
		int[] bound = getPageBoundByIndex (pageNum);
		if (index == bound[1]) {
			DestoryIcon (index);
		} else {
			GameObject lastIconOnPage = pageChildList [bound[1]];
			Location lastLocationOnPage = lastIconOnPage.GetComponent<IconController> ().originLocation;
			icon.transform.DOScale (0f, ANIM_DURATION);
			IconAdvance (index, index+1, bound[1], true);
			appList.RemoveAt (index);
			pageChildList.RemoveAt (index);
			//page 末尾插入一个占位icon
			addPlaceholder (pageNum, bound[1], lastLocationOnPage);
		}
		//清除regess与empty page 并持久化到android
		ClearEmptyPage();
		ClearRegress();
		SaveAppList ();
	}

	/// <summary>
	/// Clears the regress.
	/// </summary>
	public void ClearRegress(){
		List<GameObject> iconList = pageChildList;
		foreach(GameObject icon in iconList){
			IconController iconController = icon.GetComponent<IconController> ();
			iconController.regressLocation = null;
		}
	}


}
