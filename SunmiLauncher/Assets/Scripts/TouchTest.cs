using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchTest : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {

	const string TAG = "TouchTest";

	private LauncherController mLauncherController;



	// Use this for initialization
	void Start () {
		mLauncherController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<LauncherController>();
	}



	//listener callback

	#region IPointerDownHandler implementation
	public void OnPointerDown (PointerEventData eventData)
	{

		mLauncherController.Log (TAG, "OnPointerDown");

	}
	#endregion


	#region IPointerUpHandler implementation
	public void OnPointerUp (PointerEventData eventData)
	{

		mLauncherController.Log (TAG, "OnPointerUp");
	}
	#endregion

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData)
	{

		mLauncherController.Log (TAG, "OnPointerClick");

	}
	#endregion	


	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{

		//		Debug.Log ("OnPointerDown:"+eventData.position);
		mLauncherController.Log (TAG, "OnBeginDrag");
	}
	#endregion	

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{

		mLauncherController.Log (TAG, "OnDrag");

	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{

		//		Debug.Log ("OnEndDrag");
		mLauncherController.Log (TAG, "OnEndDrag");
	}

	#endregion
}
