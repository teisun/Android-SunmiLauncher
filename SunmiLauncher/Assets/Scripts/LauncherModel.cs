using System.Collections;

[System.Serializable]
public class LauncherModel {

	public const string V8 = "V8";//768x971
	public const string H10 = "H10";//1024x720
	public const string H14 = "H14";//1920x1008
	public const string V1 = "V1";//540x888

	public string model;
	public int currentPage = 0;
	public int pageItemNum;//
	public int rowNum;//行数
	public int columnNum;//列数
	public int screenWidth;
	public int screenHeight;
	public int itemWidth;
	public int itemHeight;
	public float iconScaleOnScreenW;
	public int iconWidth;//icon 的w与h
	public int fontSize;
	public int statusBarHeight;//status bar height
	public int navigationBarHeight;//navigation bar height
	public int proportion;

	/// <summary>
	/// proportion
	/// </summary>
	public const int SCREEN_9_16 = 1;//9:16
	public const int SCREEN_3_4 = 2;//3:4
	public const int SCREEN_4_3 = 3;//4:3
	public const int SCREEN_16_9 = 4;//16:9

}
