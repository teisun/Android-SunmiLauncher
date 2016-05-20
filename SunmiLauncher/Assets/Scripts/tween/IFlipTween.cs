using UnityEngine;

/**
 * flip tween interface
 */
public interface IFlipTween {

	void LeftSlide(float offsetX);

	void RightSlide(float offsetX);

	void LeftExit();

	void LeftEnter();

	void RightExit();

	void RightEnter();

	void LeftReset();

	void RightReset();

	void SetOrginPosition(Vector3 pos3D);

	Vector3 GetOrginPosition();

	void Reset ();

	int GetLinkagePosition ();

	void Entry();

}
