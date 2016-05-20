using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;


public class DialogController : MonoBehaviour {

	LauncherController mLauncherController;
	GameObject btnPanel;
	GameObject hint;
	string hintStr = "是否要删除此应用？\n若删除该应用，其所有数据也将被删除";
	Text hintText;

	// Use this for initialization
	void Start () {
		mLauncherController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<LauncherController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void initUninstallDialog(){
		GameObject uninstallDialog = gameObject;
		LauncherModel launcherModel = mLauncherController.launcherModel;
		RectTransform dialogRT = uninstallDialog.GetComponent<RectTransform> ();
		btnPanel = GameUtils.GetChildByName (uninstallDialog, "BtnPanel");
		RectTransform btnPanelRT = btnPanel.GetComponent<RectTransform> ();
		hint = GameUtils.GetChildByName (uninstallDialog, "hint");
		RectTransform hintRT = hint.GetComponent<RectTransform> ();
		hintText = hint.GetComponent<Text> ();
		GameObject icon = GameUtils.GetChildByName (uninstallDialog, "icon");
		RectTransform iconRT = icon.GetComponent<RectTransform> ();
		Text[] texts = btnPanel.GetComponentsInChildren<Text> ();


		Vector2 dialogSize = Vector2.zero;
		Vector2 btnPanelSize = Vector2.zero;
		Vector2 iconSize = Vector2.zero;

		Vector2 positionHint = new Vector2 (hintRT.anchoredPosition.x, Screen.height * 0.07f);
		Vector2 positionIcon = new Vector2 (iconRT.anchoredPosition.x, -Screen.height*0.02f);
		switch(launcherModel.proportion){
		case LauncherModel.SCREEN_9_16:
			dialogSize = new Vector2 (Screen.width * 0.86f, Screen.height * 0.3f);
			btnPanelSize = new Vector2 (btnPanelRT.sizeDelta.x, Screen.height * 0.07f);
			iconSize = new Vector2 (dialogSize.x * 0.2f, dialogSize.x * 0.2f);
			hintText.fontSize = 25;
			foreach(Text t in texts){
				t.fontSize = 28;
			}
			break;
		case LauncherModel.SCREEN_3_4:
			dialogSize = new Vector2 (Screen.width*0.86f, Screen.height*0.3f);
			btnPanelSize = new Vector2 (btnPanelRT.sizeDelta.x, Screen.height*0.07f);
			iconSize = new Vector2 (dialogSize.x*0.2f, dialogSize.x*0.2f);
			hintText.fontSize = 25;
			foreach(Text t in texts){
				t.fontSize = 28;
			}
			break;
		case LauncherModel.SCREEN_4_3:
			dialogSize = new Vector2 (Screen.width*0.43f, Screen.height*0.36f);
			btnPanelSize = new Vector2 (btnPanelRT.sizeDelta.x, Screen.height*0.09f);
			iconSize = new Vector2 (dialogSize.x*0.21f, dialogSize.x*0.21f);
			hintText.fontSize = 19;
			foreach(Text t in texts){
				t.fontSize = 22;
			}
			break;
		case LauncherModel.SCREEN_16_9:
			dialogSize = new Vector2 (Screen.width*0.3f, Screen.height*0.32f);
			btnPanelSize = new Vector2 (btnPanelRT.sizeDelta.x, Screen.height*0.08f);
			iconSize = new Vector2 (dialogSize.x*0.21f, dialogSize.x*0.21f);
			hintText.fontSize = 23;
			foreach(Text t in texts){
				t.fontSize = 26;
			}
			break;
		default:
			dialogSize = new Vector2 (Screen.width*0.86f, Screen.height*0.3f);
			btnPanelSize = new Vector2 (btnPanelRT.sizeDelta.x, Screen.height*0.07f);
			hintText.fontSize = 25;
			break;
		}

		dialogRT.sizeDelta = dialogSize;
		btnPanelRT.sizeDelta = btnPanelSize;
		hintRT.anchoredPosition = positionHint;
		iconRT.anchoredPosition = positionIcon;
		iconRT.sizeDelta = iconSize;

	}

	public void show(App app){
		btnPanel.SetActive (true);
		hintText.text = hintStr;
		RectTransform dialogRT = GetComponent<RectTransform> ();
		dialogRT.DOScale (1f, LauncherController.ANIM_DURATION);
		GameObject icon = GameUtils.GetChildByName (gameObject, "icon");
		Image img = icon.GetComponent<Image> ();
		Sprite sprite = mLauncherController.LoadImgByIO (app);
		img.sprite = sprite;
	}

	public void cancel(){
		RectTransform dialogRT = GetComponent<RectTransform> ();
		dialogRT.DOScale (0f, LauncherController.ANIM_DURATION);
	}

	public void progress(){
		btnPanel.SetActive (false);
		hintText.text = "卸载中...请稍等";
	}

	public void uninstalled(){
		hintText.text = "卸载成功";
		cancel ();
	}
}
