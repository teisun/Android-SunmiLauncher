package sunmi.launcher;

import java.io.FileNotFoundException;
import java.io.PrintStream;
import java.lang.Thread.UncaughtExceptionHandler;

import sunmi.launcher.utils.FileUtils;
import android.app.Application;
import android.util.Log;

public class SunmiApplication extends Application {

	protected static final String TAG = "SunmiApplication";

	@Override
	public void onCreate() {
		super.onCreate();
		configUncaughtExceptionHandler();
	}


	/** 捕获异常 */
	private void configUncaughtExceptionHandler() {
		Thread.setDefaultUncaughtExceptionHandler(new UncaughtExceptionHandler() {

			@Override
			public void uncaughtException(Thread thread, Throwable ex) {
				Log.e(TAG, "uncaughtException crash", ex);
				try {
					ex.printStackTrace(new PrintStream(FileUtils.createErrorFile()));
				} catch (FileNotFoundException e) {
					Log.e(TAG, "创建异常文件失败");
					e.printStackTrace();
				}
				android.os.Process.killProcess(android.os.Process.myPid());
				System.exit(0);
			}

		});
	}
	
	
	
}