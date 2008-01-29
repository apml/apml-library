module Bitnik
  module Apm
    #Class that implements an algorithm to build an APML file from an AttentionSource object.
    class Builder

      attr_reader :version
      attr_accessor :title
      attr_accessor :generator
      attr_accessor :user_email
      attr_accessor :default_profile
      attr_accessor :from

      #Constructor
      #Parameters:
      #- calculator an object that implements the method compute_attention(concept,mode,value,total_attention_value) used
      #             to calculate the attention APML attribute for an AttentionDestiny or AttentionDestinyReference.
      #             If none is provided, the proportional value of each destiny will be used as the value for the implicit
      #             concepts and sources. The sum of the attention points will be used for explicit elements.
      def initialize(calculator=nil)
        @version="0.6"
        @title="APML file"
        @generator = "apm bintik ruby on rails plugin"
        @default_profile = nil
        @from = "bitnik.apml.plugin"
        if (calculator==nil)
          @calculator = self
        else
          @calculator = calculator
        end

      end

      #Builds the APML file for an AttentionSource object.
      def build_profile_for(attention_consumer)
        xml = StringIO.new

        xml << '<?xml version="1.0"?>'
        xml << "<APML xmlns=\"http://www.apml.org/apml-"+@version+"\" version=\""+@version+"\" >"
        xml << "<Head>"
        xml << "<Title>#{@title}</Title>"
        xml << "<Generator>#{@generator}</Generator>"
        if(@user_email!=nil)
          xml << "<User-Email>#{user_email}</User-Email>"
        end
        xml << "<DateCreated>#{Time.now.xmlschema}</DateCreated>"
        xml << "</Head>"
        if(@default_profile!=nil)
          xml << "<Body defaultprofile=\"#{@default_profile}\">"
        else
          xml << "<Body>"
        end
        total_value = attention_consumer.compute_total_implicit_attention_value
        attention_consumer.attention_profiles.each do |profile|
          xml << "<Profile name=\"#{profile.name}\">"
          @implicit_mapping = attention_consumer.build_implicit_attention_profile_mapping(profile)
          xml << "<ImplicitData><Concepts>"
          @implicit_mapping.each_pair do |concept,value|
            computed = @calculator.compute_attention(concept,:implicit,value,total_value)
            if(concept.attention_destiny.from==nil)
              from = @from
            else
              from = concept.attention_destiny.from
            end
            updated = concept.attention_destiny.updated_at.xmlschema
            name = concept.attention_destiny.key
            xml << "<Concept key=\"#{name}\" value=\"#{computed}\" from=\"#{from}\" updated=\"#{updated}\" />"
          end
          xml << "</Concepts>"
          @implicit_references_mapping = attention_consumer.build_implicit_attention_reference_profile_mapping(profile)
          xml << "<Sources>"
          @implicit_references_mapping.each_pair do |ref,value|
            computed = @calculator.compute_attention(ref,:implicit,value,total_value)
            from = ref.from
            key = ref.key
            name = ref.name
            type = ref.mime_type
            updated = ref.updated_at.xmlschema
            xml << "<Source"
            if(key!=nil)
              xml << " key=\"#{key}\""
            end
            if(name!=nil)
              xml << " name=\"#{name}\""
            end
            if(type!=nil)
              xml << " type=\"#{type}\""
            end
            if(from!=nil)
              xml << " from=\"#{from}\""
            end
            xml << " value=\"#{computed}\""
            xml << " updated=\"#{updated}\">"
            author = ref.author
            key = author.key
            value = author.value
            from = author.value
            if(author.updated_at!=nil)
              updated = author.updated_at.xmlschema
            else
              updated=nil
            end

            xml << "<Author"
            if(key!=nil)
              xml << " key=\"#{key}\""
            end
            if(value!=nil)
              xml << " value=\"#{value}\""
            end
            if(from!=nil)
              xml << " from=\"#{from}\""
            end
            if(updated!=nil)
              xml << " updated=\"#{updated}\""
            end
            xml << " />"

            xml << "</Source>"
          end
          xml << "</Sources>"
          xml << "</ImplicitData>"

          @explicit_mapping = attention_consumer.build_explicit_attention_profile_mapping(profile)
          xml << "<ExplicitData><Concepts>"
          @explicit_mapping.each_pair do |concept,value|
            #computed = @calculator.compute_attention(concept,:explicit,value,total_value)
            if(concept.attention_destiny.from==nil)
              from = @from
            else
              from = concept.attention_destiny.from
            end
            updated = concept.attention_destiny.updated_at.xmlschema
            name = concept.attention_destiny.key
            xml << "<Concept key=\"#{name}\" value=\"#{value}\"/>"
          end
          xml << "</Concepts>"

          @explicit_references_mapping = attention_consumer.build_explicit_attention_reference_profile_mapping(profile)
          xml << "<Sources>"
          @explicit_references_mapping.each_pair do |ref,value|
            from = ref.from
            key = ref.key
            name = ref.name
            type = ref.mime_type
            updated = ref.updated_at.xmlschema
            xml << "<Source"
            if(key!=nil)
              xml << " key=\"#{key}\""
            end
            if(name!=nil)
              xml << " name=\"#{name}\""
            end
            if(type!=nil)
              xml << " type=\"#{type}\""
            end
            xml << " value=\"#{value}\""
            xml << ' >'

            author = ref.author
            key = author.key
            value = author.value
            from = author.value
            if(author.updated_at!=nil)
              updated = author.updated_at.xmlschema
            else
              updated=nil
            end

            xml << "<Author"
            if(key!=nil)
              xml << " key=\"#{key}\""
            end
            if(value!=nil)
              xml << " value=\"#{value}\""
            end
            xml << " />"

            xml << "</Source>"
          end
          xml << "</Sources>"

          xml << "</ExplicitData>"
          xml << "</Profile>"
        end
        xml << "</Body>"
        xml << "</APML>"

        return xml.string
      end

      #Default attention calcule algorithm.
      def compute_attention(concept,mode,value,total_attention_value)
        if(total_attention_value!=0)
          value.to_f/total_attention_value.to_f
        else
          0
        end
      end

    end
  end
end
