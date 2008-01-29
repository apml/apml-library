require 'automata_parser/stack'
require 'automata_parser/graph'
require 'automata_parser/parser'

module Bitnik
  module Apm

    include AutomataParser

    #Author APML proxy class.
    class Author
      attr_accessor :key
      attr_accessor :from
      attr_accessor :updated
    end

    #Concept APML proxy class.
    class Concept
      attr_accessor :key
      attr_accessor :value
      attr_accessor :from
      attr_accessor :updated
    end

    #Source APML proxy class.
    class Source
      attr_accessor :key
      attr_accessor :name
      attr_accessor :value
      attr_accessor :type
      attr_accessor :from
      attr_accessor :updated
      attr_accessor :author
    end

    #Profile APML proxy class.
    class Profile
      attr_accessor :name
      attr_accessor :explicit_concepts
      attr_accessor :explicit_sources
      attr_accessor :implicit_concepts
      attr_accessor :implicit_sources

      def initialize
        @explicit_concepts = Hash.new
        @implicit_concepts = Hash.new
        @implicit_sources = Hash.new
        @explicit_sources = Hash.new
      end
    end

    #Models the structure of an APML file.
    class ApmlDocument

      #APML Version element.
      attr_accessor :version
      #APML Title element.
      attr_accessor :title
      #APML Generator element.
      attr_accessor :generator
      #APML UserEmail element.
      attr_accessor :user_email
      #APML DateCreated element.
      attr_accessor :date_created
      #APML Profile element
      attr_accessor :profiles

      #Constructor
      #The APML XML stream is passed as a parameter.
      def initialize(xml)
        @mapping = [[:init,:apml,"APML"],
                   [:apml,:head,"Head"],
                   [:head,:title,"Title"],
                   [:title,:head,"/Title"],
                   [:head,:generator,"Generator"],
                   [:generator,:head,"/Generator"],
                   [:head,:user_email,"UserEmail"],
                   [:user_email,:head,"/UserEmail"],
                   [:head,:date_created,"DateCreated"],
                   [:date_created,:head,"/DateCreated"],
                   [:head,:apml,"/Head"],
                   [:apml,:body,"Body"],
                   [:body,:profile,"Profile"],
                   [:profile,:implicit_data,"ImplicitData"],
                   [:implicit_data,:implicit_concept,"Concept"],
                   [:implicit_concept,:implicit_data,"/Concept"],
                   [:implicit_data,:implicit_source, "Source"],
                   [:implicit_source,:implicit_data,"/Source"],
                   [:implicit_source,:author,"Author"],
                   [:author,:implicit_source,"/Author"],
                   [:implicit_data,:profile,"/ImplicitData"],
                   [:profile,:explicit_data,"ExplicitData"],
                   [:explicit_data,:explicit_concept,"Concept"],
                   [:explicit_concept,:explicit_data,"/Concept"],
                   [:explicit_data,:source_explicit, "Source"],
                   [:source_explicit,:explicit_data,"/Source"],
                   [:explicit_source,:author,"Author"],
                   [:author,:explicit_source,"/Author"],
                   [:explicit_data,:profile,"/ExplicitData"],
                   [:profile,:body,"/Profile"],
                   [:body,:apml,"/APML"]]

        @profiles = Hash.new
        @parser = AutomataParser::Parser.new(@mapping,:init,self)
        @parser.parse xml
      end

      #Parsing method
      def found_APML(apml)
        @version = apml.values["version"]
      end

      #Parsing method
      def parsed_Title(title)
        @title = title.text
      end

      #Parsing method
      def parsed_Generator(generator)
        @generator = generator.text
      end

      #Parsing method
      def parsed_UserEmail(email)
        @user_email = email.text
      end

      #Parsing method
      def parsed_DateCreated(date)
        @date_created = Time.parse(date.text)
      end

      #Parsing method
      def found_Profile(profile)
        @current_profile = Profile.new
        @current_profile.name = profile.values["name"]
      end

      #Parsing method
      def parsed_Profile(profile)
        @profiles[@current_profile.name] =  @current_profile
      end

      #Parsing method
      def parsed_Concept(concept)
        oconcept = Concept.new
        oconcept.key = concept.values["key"]
        oconcept.value = concept.values["value"]
        if @parser.current_state == :implicit_data
          oconcept.from = concept.values["from"]
          oconcept.updated = Time.parse(concept.values["updated"])
          @current_profile.implicit_concepts[oconcept.key] = oconcept
        else
          @current_profile.explicit_concepts[oconcept.key] = oconcept
        end
      end

      #Parsing method
      def parsed_Author(author)
        oauthor = Author.new
        oauthor.key = author.values["key"]
        oauthor.from = author.values["from"]
        oauthor.updated = Time.parse(author.values["updated"])
        @current_source.author = oauthor
      end

      #Parsing method
      def found_Source(source)
        @current_source = Source.new
      end

      #Parsing method
      def parsed_Source(source)
        begin
        @current_source.key = source.values["key"]
        @current_source.value = source.values["value"]
        @current_source.name = source.values["name"]
        @current_source.type = source.values["type"]
        @current_source.from = source.values["from"]
        if source.values["updated"]!=nil
          @current_source.updated = Time.parse(source.values["updated"])
        end
        if @parser.current_state == :implicit_data
          @current_profile.implicit_sources[@current_source.key] = @current_source
        else
          @current_profile.explicit_sources[@current_source.key] = @current_source
        end
        rescue Exception => ex
          puts "EXCEPCION#{ex.message}"
          puts ex.backtrace.join("\n")
        end
      end
    end

  end
end
