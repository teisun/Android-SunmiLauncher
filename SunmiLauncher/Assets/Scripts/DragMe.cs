using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class DragMe : MonoBehaviour
{

	const string TAG = "DragMe";
	public LauncherController mLauncherController;
	public LauncherModel launcherModel;

	public bool dragOnSurfaces = true;
	
	private Dictionary<int,GameObject> m_DraggingIcons = new Dictionary<int, GameObject>();
	private Dictionary<int, RectTransform> m_DraggingPlanes = new Dictionary<int, RectTransform>();

	private Vector2 offset;//离物体右下角的距离

	#region IPointerDownHandler implementation

	public void OnPointerDown (PointerEventData eventData)
	{
		mLauncherController.Log (TAG, "OnPointerDown eventData.position:"+eventData.position);
		RectTransform rt = GetComponent<RectTransform> ();
		float x = eventData.position.x - rt.anchoredPosition.x;
		float y = eventData.position.y - rt.anchoredPosition.y;
		offset = new Vector2 (x, y);

		var dragIconParent = FindInParents<PageController>(gameObject);
		if (dragIconParent == null)
			return;

		// We have clicked something that can be dragged.
		// What we want to do is create an icon for this.
		m_DraggingIcons[eventData.pointerId] = gameObject;

		m_DraggingIcons[eventData.pointerId].transform.SetParent (dragIconParent.transform, false);
		m_DraggingIcons[eventData.pointerId].transform.SetAsLastSibling();
	}

	#endregion

	public void OnBeginDrag(PointerEventData eventData)
	{
		mLauncherController.Log (TAG, "OnBeginDrag eventData.position:"+eventData.position);
		var canvas = FindInParents<PageController>(gameObject);
		if (canvas == null)
			return;
//		var image = m_DraggingIcons[eventData.pointerId].AddComponent<Image>();
		// The icon will be under the cursor.
		// We want it to be ignored by the event system.
//		var group = m_DraggingIcons[eventData.pointerId].AddComponent<CanvasGroup>();
//		group.blocksRaycasts = false;

//		image.sprite = GetComponent<Image>().sprite;
//		image.SetNativeSize();
		
		if (dragOnSurfaces)
			m_DraggingPlanes[eventData.pointerId] = transform as RectTransform;
		else
			m_DraggingPlanes[eventData.pointerId]  = canvas.transform as RectTransform;
		
		SetDraggedPosition(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		mLauncherController.Log (TAG, "OnDrag eventData.position:"+eventData.position);
		if (m_DraggingIcons[eventData.pointerId] != null)
			SetDraggedPosition(eventData);
	}

	private void SetDraggedPosition(PointerEventData eventData)
	{
		if (dragOnSurfaces && eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
			m_DraggingPlanes[eventData.pointerId] = eventData.pointerEnter.transform as RectTransform;
		
		RectTransform rt = m_DraggingIcons[eventData.pointerId].GetComponent<RectTransform>();

		CorrectingUtils.PointerEventCorrecting (ref eventData, launcherModel.navigationBarHeight);
		Vector3 globalMousePos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlanes[eventData.pointerId], eventData.position-offset, eventData.pressEventCamera, out globalMousePos))
		{
			rt.position = globalMousePos;
			rt.rotation = m_DraggingPlanes[eventData.pointerId].rotation;

			Vector3 pos3D = new Vector3 (rt.anchoredPosition3D.x, rt.anchoredPosition3D.y, LauncherController.ICON_RISING_DISTANCE);
			rt.anchoredPosition3D = pos3D;
			Debug.Log ("rt.position:"+rt.position);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
//		if (m_DraggingIcons[eventData.pointerId] != null)
//			Destroy(m_DraggingIcons[eventData.pointerId]);
//		Destroy(GetComponent<CanvasGroup>());
		m_DraggingIcons[eventData.pointerId] = null;

	}

	static public T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null) return null;
		var comp = go.GetComponent<T>();

		if (comp != null)
			return comp;
		
		var t = go.transform.parent;
		while (t != null && comp == null)
		{
			comp = t.gameObject.GetComponent<T>();
			t = t.parent;
		}
		return comp;
	}
}
