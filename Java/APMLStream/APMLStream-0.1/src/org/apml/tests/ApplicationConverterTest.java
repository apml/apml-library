package org.apml.tests;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import org.apml.base.Application;
import org.apml.base.Applications;
import org.apml.converters.ApplicationsConverter;
import com.thoughtworks.xstream.XStream;
import com.thoughtworks.xstream.io.xml.DomDriver;
import com.thoughtworks.xstream.io.xml.XppDomDriver;
import com.thoughtworks.xstream.io.xml.XppDriver;

public class ApplicationConverterTest
{
	public static void main(String[] args)
	{
	    File f = null;
	    BufferedReader in = null;
	    String lin = "";
	    String payload = "";
	    XStream xstream = new XStream();
	    ApplicationsConverter appConv = new ApplicationsConverter(xstream.getMapper());
		xstream.registerConverter(appConv);
		xstream.alias("Applications",Applications.class);
		xstream.alias("Application", Application.class);

		try
		{
			f = new File(args[0]);
			in = new BufferedReader(new FileReader(f));

			while ((lin = in.readLine()) != null)
				payload += lin;

			Applications<Application> apps = (Applications) xstream.fromXML(payload);
		}
		catch(Exception ex)
		{
			ex.printStackTrace();
		}
	}
}
