using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public interface LongClickListener{
	void OnLongClick(PointerEventData eventData);
}

public class LongClickUtils{


	public LongClickListener longClickListener;

	float interval = 0.1f;
	float longPressDelay = 1.0f;

	bool isTouchDown = false;
	public bool isLongPress = false;
	public bool callbacked = false;
	float touchBegin = 0;
	float lastInvokeTime = 0; 

	public PointerEventData lastEventData;

	// Update is called once per frame
	public void Update () {
		if (isTouchDown) {
			if (isLongPress) {
				if (Time.time - lastInvokeTime > interval) {
					if (longClickListener!=null&&!callbacked) {
						callbacked = true;
						longClickListener.OnLongClick (lastEventData);
					}
					lastInvokeTime = Time.time;
				}
			} else {
				if (Time.time - touchBegin > longPressDelay) { 
					isLongPress = true;
				}
			}
		}
	}



	public void OnPointerDown (PointerEventData eventData)
	{
		touchBegin = Time.time;
		isTouchDown = true;
		lastEventData = eventData;
	}


	public void OnPointerExit (PointerEventData eventData)
	{
		cancel ();
	}


	public void OnPointerUp (PointerEventData eventData)
	{
		cancel ();
	}

	public void cancel(){
		isTouchDown = false;
		isLongPress = false;
		callbacked = false;
	}
}
