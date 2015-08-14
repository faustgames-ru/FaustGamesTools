/*
 * Copyright (C) 2009 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.faustgames.testjni;

import android.app.Activity;
import android.widget.TextView;
import android.os.Bundle;

/**
 * This class loads the Java Native Interface (JNI)
 * library, 'libAndroid1.so', and provides access to the
 * exposed C functions.
 * The library is packaged and installed with the application.
 * See the C file, /jni/TestJni.c file for the
 * implementations of the native methods. 
 * 
 * For more information on JNI, see: http://java.sun.com/docs/books/jni/
 */

public class TestJni extends Activity
{
	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);

		// Enter code here
	}

	/**
	 * An example native method.  See the library function,
	 * <code>Java_com_faustgames_testjni_TestJni_testjniNative</code>
	 * for the implementation.
	 */
	public native void testjniNative();

	/* This is the static constructor used to load the
	 * 'Android1' library when the class is
	 * loaded.
	 */
	static {
		System.loadLibrary("Android1");
	}
}
