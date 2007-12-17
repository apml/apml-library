package org.apml.deserialize.tests;

import java.util.Iterator;

import org.apml.deserialize.parser.APMLParser;
import org.apml.deserialize.exceptions.ProfileDoesNotExistException;
import org.apml.deserialize.model.*;;

public class APMLTest
{
	public static void main(String[] args)
	{
		System.out.println("APML Specification Version: " + APMLParser.APML_SPEC_VERSION);
		
		long start = System.currentTimeMillis();
		
		// ****************************************
		// Deserialize the file into an APML object
		// ****************************************
		APML apml = new APMLParser("./temp/example.apml.xml").deserialize();
		
		// ******************************************
		// Gets Head and Body information of the file
		// ******************************************
		Head header = apml.getHead();
		Body body = apml.getBody();
		
		// ***********************************
		// Gets all profiles found in the Body
		// ***********************************
		Profiles allProfiles = body.getProfiles();
		
		// ***********************
		// Gets a specific Profile
		// ***********************
		Profile profile = null;
		try
		{
			profile = allProfiles.getProfile("Home");
		}
		catch(ProfileDoesNotExistException pEx)
		{
			System.out.println(pEx.toString());
		}
		
		// **********************************************
		// Gets the Implicit and Explicit data structures
		// **********************************************
		ImplicitData implicitData = profile.getImplicitData();
		ExplicitData explicitData = profile.getExplicitData();
		
		// **************************************
		// Gets all Implicit Concepts and Sources
		// **************************************
		Concepts concepts = implicitData.getConcepts();
		Sources sources = implicitData.getSources();
		
		// *****************************************************
		// Iterate through all ImplicitData Concepts and display
		// *****************************************************
		Iterator iConcepts = concepts.iterator();
		int i = 0;
		while(iConcepts.hasNext())
		{
			Concept concept = (Concept) iConcepts.next();
			System.out.println(concept.getKey() + "," + concept.getValue() + "," + concept.getURI() + "," + concept.getFrom() + "," + concept.getUpdated());
			i++;
		}
		long finish = System.currentTimeMillis();
		System.out.println("Processed " + i + " Concepts in " + ((finish - start) / 1000.00) + " seconds");
		
		// ************************************************
		// Iterate through the Implicit Sources and display
		// ************************************************
		Iterator iSources = sources.iterator();
		while(iSources.hasNext())
		{
			Source source = (Source) iSources.next();
			System.out.println(source.getKey() + "," + source.getValue() + "," + source.getFrom() + "," + source.getType() + "," + source.getUpdated());
			// Gets the source author
			Author author = source.getAuthor();
			System.out.println("Author: " + author.getKey());
		}
	}
}
