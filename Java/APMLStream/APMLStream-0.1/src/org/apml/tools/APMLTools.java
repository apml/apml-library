/*	
 *	APMLStream: Java library for APML serialization/deserialization
 *	By:  Tim Schultz
 *
 *	Licensed under the Apache License, Version 2.0 (the "License"); 
 *	you may not use this file except in compliance with the License. 
 *	You may obtain a copy of the License at 
 *
 *	http://www.apache.org/licenses/LICENSE-2.0 
 *
 *	Unless required by applicable law or agreed to in writing, software 
 *	distributed under the License is distributed on an "AS IS" BASIS, 
 *	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 *	See the License for the specific language governing permissions and 
 *	limitations under the License.
 *
 */

package org.apml.tools;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.text.SimpleDateFormat;
import java.util.Date;

/**
 * <p>Various helper methods useful for manipulating APML objects.</p>
 * @author Tim Schultz
 */
public class APMLTools
{
	/**
	 * Determines if a resource specified by a ISO8601 date is outdated
	 * @param date
	 * @return outdated
	 */
	public static boolean isResourceOutdated(String date)
	{
		// TODO: Parse and process date
		return false;
	}
	
	/**
	 * Converts a default Data object to ISO8601DateType
	 * @param date
	 * @return The ISO8601 compatible date
	 */
	public static String convertToISO8601DateType(Date date)
	{
		return new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'").format(date);
	}
	
	/**
	 * Converts UserEmail text to MD5 for security purposes
	 * @param email
	 * @return The MD5 hash of the user's email
	 */
	public String userEmailMD5(String email)
	{        
		byte[] bytes = email.getBytes();
		byte mDigest[] = null;
		MessageDigest algorithm = null;
		try
		{
			algorithm = MessageDigest.getInstance("MD5");
			algorithm.reset();
			algorithm.update(bytes);
			mDigest = algorithm.digest();
		            
			StringBuffer hexString = new StringBuffer();
			int i = 0;
			for (i=0;i<mDigest.length;i++)
				hexString.append(Integer.toHexString(0xFF & mDigest[i]));
		}
		catch(NoSuchAlgorithmException nsae){}
		return mDigest.toString();
	}
}
