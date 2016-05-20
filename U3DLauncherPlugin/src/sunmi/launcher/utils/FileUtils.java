package sunmi.launcher.utils;

import java.io.File;
import java.text.DecimalFormat;

import android.annotation.SuppressLint;
import android.os.Environment;

@SuppressLint("NewApi")
public class FileUtils {

	private static final String TAG = "FileUtils";
	
	private static final int ERROR = -1;

	/**
	 * 判断是否存在sd卡
	 * 
	 * @return
	 */
	public static boolean isSDCARDMounted() {
		String status = Environment.getExternalStorageState();
		if (status.equals(Environment.MEDIA_MOUNTED))
			return true;
		return false;
	}
	
	public static String getSDCardPath(){
		return Environment.getExternalStorageDirectory().getPath();
	}



	/**
	 * 创建文件夹
	 * 
	 * @param folderName
	 */
	public static boolean createFolder(String folderName) {
		for (int i = 0; i < 3; i++) {
			File file = new File(folderName);
			if (!file.exists()) { // 文件不存在
				// 创建临时文件夹
				boolean mkdir = file.mkdir();
				if (mkdir) {
					return true;
				} else {
					LogUtil.e(TAG, "创建文件夹失败" + folderName);
				}
			} else {
				if (!file.isDirectory()) {
					file.delete();
					boolean mkdir = file.mkdir();
					if (mkdir) {
						return true;
					} else {
						LogUtil.e(TAG, "创建文件夹失败");
					}
				} else {
					return true;
				}
			}
		}
		return false;

	}

	/**
	 * 创建文件
	 * 
	 * @param fileName
	 * @return
	 */
	public static boolean createFile(String fileName) {
		File file = new File(fileName);
		if (file.exists() && file.isFile()) {
			return true;
		}
		return false;
	}

	/**
	 * 删除临时文件
	 */
	public static void deleteFile(String fileName) {
		File _file = new File(fileName);
		if (_file.isDirectory()) {
			File[] tempFiles = getTempFiles(fileName);
			if(tempFiles ==null )return;
			for (File file : tempFiles) {
				file.delete();
			}
		} else {
			_file.delete();
		}

	}

	/**
	 * 获得临时文件夹的文件列表
	 * 
	 * @return
	 */
	public static File[] getTempFiles(String folderName) {
		File folder = new File(folderName);
		return folder.listFiles();
	}

	public static int fileSize(String folderName) {
		return getTempFiles(folderName).length;
	}
	
	/**
	 * 根据文件path得到文件大小
	 */
	public static long getFileSizeByFilePath(String path){
		long size;
		File file = new File(path);
		if (file.exists()) {
			size = file.length();
		} else {
			size = 0;
		}
		return size;
	}

	/**
	 * 
	 * bit装换成MB
	 * 
	 * @return
	 */

	public static String FileSize(float FileBitSize) {
		String fileSize = null;
		DecimalFormat df = new DecimalFormat("0.0");
		fileSize = df.format(FileBitSize / (1024 * 1024)) + "M";
		return fileSize;
	}
	
	/**创建异常文件*/
	public static File createErrorFile(){
		File file = null;
		try {
			String DIR = Environment.getExternalStorageDirectory().getAbsolutePath() + "/LauncherPlugin/log/";
			String NAME = DateUtils.getCurrentDateString() + ".txt";
			File dir = new File(DIR);
			if (!dir.exists()) {
				dir.mkdirs();
			}
			file = new File(dir, NAME);
		} catch (Exception e) {
			e.printStackTrace();
		}
		return file;
	}
	
	/**
	 * 删除一个文件
	 * @param file
	 */
	public static void deleteFile(File file){
		File to = new File(file.getAbsolutePath() + System.currentTimeMillis());
		file.renameTo(to);
		to.delete();
	}
	
}
