require 'active_record'
require 'apm_builder'

module Bitnik
  module Acts
    module AttentionSourceLib

      def self.included(mod)
        mod.extend(ClassMethods)
      end

      module ClassMethods
        mattr_accessor :bitnik_apm_key_mapping
        mattr_accessor :bitnik_apm_from_value

        #Defines this model to be an attention source.
        #Attention sources generate attention values that can be assigned to AttentionDestiny objects
        #and AttentionDestinyReference objects, in the form of several AttentionPoint objects.
        #An AttentionSource has several associated AttentionProfile objects, equivalent to the <Profile> APML
        #specification element. Each source has a default profile associated whe it is created.
        #You can transform an AttentionSource object into an XML representation of his attention profiles, that
        #conforms to the APML specification.
        def acts_as_attention_source
          extend SingletonMethods
          include InstanceMethods

          has_many :attention_profiles, :as => :attention_source, :dependent => :destroy

          after_save :setup_default_attention_profile

          extend Bitnik::Acts::AttentionSourceLib::SingletonMethods
          include Bitnik::Acts::AttentionSourceLib::InstanceMethods
        end
      end


      module SingletonMethods
      end

      module InstanceMethods
        #Assigns a value to the the attention an AttentionSource object pays to an AttentionDestiny
        #or AttentionDestinyReference object.
        #Parameters:
        #[+attention_destiny+] An AttentionDestiny or AttentionDestinyReference object deserving the attention of the AttentionSource.
        #[+attention_value+]  A representation of the value fo the attention the source is paying to the destiny.
        #[+mode+] Whether it is :explicit or :implicit attention. See the APML specification for more details.
        #[+attention_profile+] The context in which this source is paying attention the destiny. By default it will be the default profile.
        def pay_attention_to(attention_destiny,attention_value,mode,attention_profile=nil)
          attention_profile = profile_or_default(attention_profile)
          destiny = check_destiny(attention_destiny)

          if(destiny.class==AttentionDestiny)
            points = AttentionPoint.new
            points.attention_destiny = destiny
          else
            points = AttentionReferencePoint.new
            points.attention_destiny_reference = destiny
          end

          points.mode = mode.to_s
          points.attention_profile = attention_profile
          points.value = attention_value

          points.save!

        end

        #Equivalent to a call to pay_attention_to with an :explicit value for the mode parameter.
        def pay_explicit_attention_to(attention_destiny, attention_value,attention_profile=nil)
          pay_attention_to(attention_destiny,attention_value,:explicit,attention_profile)
        end

        #Equivalent to a call to pay_attention_to with an :implicit value for the mode parameter.
        def pay_implicit_attention_to(attention_destiny, attention_value,attention_profile=nil)
          pay_attention_to(attention_destiny,attention_value,:implicit,attention_profile)
        end

        #Associates a new profile with profile_name name value to this AttentionSource.
        def create_attention_profile(profile_name)
          origin = AttentionProfile.new
          origin.name = profile_name.to_s
          origin.attention_source = self
          origin.save!
        end

        #Removes an attention profile of name profile_name from this AttentionSource.
        def drop_attention_profile(profile_name)
          attention_profile = AttentionProfile.find(:first, :conditions => ["name=? AND attention_source_id=? AND attention_source_type=?",profile_name.to_s,self.id,self.class.to_s])
          attention_profile.destroy
        end

        #Returns all the AttentionDestiny object, implicit and explicit, this attention source has payed attention to.
        #Parameters:
        #[+mode+] :implicit or :explicit attention.
        #[+profile+] the profile where to look for attention. By default the 'default' profile is used.
        def attention_destinies(mode,profile=nil)
          profile = profile_or_default(profile)
          AttentionDestiny.find_by_sql(["select d.* from attention_destinies as d, attention_points as p WHERE p.attention_profile_id=? and p.mode=? and d.id=p.attention_destiny_id group by d.id",profile.id,mode.to_s]).collect{ |dst| dst.attention_generator}
        end

        #Returns only the explicit attention destinies for this source.
        def explicit_attention_destinies(profile=nil)
          attention_destinies(:explicit,profile)
        end

        #Returns only the implicit attention destinies for this source.
        def implicit_attention_destinies(profile=nil)
          attention_destinies(:implicit,profile)
        end

        #Returns all the AttentionDestinyReference objects, implicit and explicit, this attention source has payed attention to.
        def attention_reference_destinies(mode,profile=nil)
          profile = profile_or_default(profile)
          AttentionDestinyReference.find_by_sql(["select d.* from attention_destiny_references as d, attention_reference_points as p WHERE p.attention_profile_id=? and p.mode=? and d.id=p.attention_destiny_reference_id group by d.id",profile.id,mode.to_s])
        end

        #Returns only the explicit attention destiny references for this source.
        def explicit_attention_reference_destinies(profile=nil)
          attention_reference_destinies(:explicit,profile)
        end

        #Returns only the implicit attention destiny references for this source.
        def implicit_attention_reference_destinies(profile=nil)
          attention_reference_destinies(:implicit,profile)
        end

        #Returns all the AttentionPoint objects this source has assigned to a given attention destiny.
        #Parameters:
        #[+destiny+] AttentionDestiny or AttentionDestinyReference object to check for attention points.
        #[+mode+] Check for :explicit or :implicit attention points.
        #[+profile+] The profile where to look for attention points.
        def attention_points_for(destiny,mode,profile=nil)
          if(destiny.class!=AttentionDestinyReference)
            profile = profile_or_default(profile)
            points = AttentionPoint.find_by_sql(["select  p.* from attention_destinies as d, attention_points as p WHERE p.attention_profile_id=? and p.mode=? and d.id=p.attention_destiny_id and d.attention_generator_id=? and d.attention_generator_type=?",profile.id,mode.to_s,destiny.id,destiny.class.to_s])
            return points
          else
            profile = profile_or_default(profile)
            points = AttentionReferencePoint.find_by_sql(["select  p.* from attention_destiny_references as d, attention_reference_points as p WHERE p.attention_profile_id=? and p.mode=? and d.id=p.attention_destiny_reference_id and d.id=?" ,profile.id,mode.to_s,destiny.id])
            return points
          end
        end

        #Equivalent to a call to attention_points_for with an :explicit value for the mode parameter.
        def explicit_attention_points_for(destiny,profile=nil)
          attention_points_for(destiny,:explicit,profile)
        end

        #Equivalent to a call to attention_points_for with an :implicit value for the mode parameter.
        def implicit_attention_points_for(destiny,profile=nil)
          attention_points_for(destiny,:implicit,profile)
        end

        #Computes the total attention value (implicit and explicit) for an AttentionDestiny for a given mode and profile.
        def attention_total_value_for(destiny,mode,profile=nil)
          count = 0
          attention_points_for(destiny,mode,profile).each do |point|
            count += point.value
          end
          return count
        end

        #Computes the total explicit value for an AttentionDestiny for a profile
        def explicit_attention_total_value_for(destiny,profile=nil)
          attention_total_value_for(destiny,:explicit,profile)
        end

        #Computes the total implicit value for an AttentionDestiny for a given mode and profile.
        def implicit_attention_total_value_for(destiny,profile=nil)
          attention_total_value_for(destiny,:implicit,profile)
        end

        #Builds a hash map for the attention destinies of the source for a given mode and profile.
        #The built map will ben of the form { AttentionDestiny object => value }
        #Parameters:
        #[+mode+] :implicit or :explicit attention.
        #[+profile+] the profile where to look for attention. By default, 'default' will be used.
        def build_attention_profile_mapping(mode,profile=nil)
          map = Hash.new
          attention_destinies(mode,profile).each do |destiny|
            map[destiny] = attention_total_value_for(destiny,mode,profile)
          end

          return map
        end

        #Equivalent to build_attention_profile_mapping with an :explicit value for the mode parameter.
        def build_explicit_attention_profile_mapping(profile=nil)
          build_attention_profile_mapping(:explicit,profile)
        end

        #Equivalent to build_attention_profile_mapping with an :implicit value for the mode parameter.
        def build_implicit_attention_profile_mapping(profile=nil)
          build_attention_profile_mapping(:implicit,profile)
        end

        #Builds a hash map for the attention destiny references of the source for a given mode and profile.
        #The built map will ben of the form { AttentionDestiny object => value }
        #Parameters:
        #[+mode+] :implicit or :explicit attention.
        #[+profile+] the profile where to look for attention. By default, 'default' will be used.
        def build_attention_reference_profile_mapping(mode,profile=nil)
          map = Hash.new

          attention_reference_destinies(mode,profile).each do |ref|
            map[ref] = attention_total_value_for(ref,mode,profile)
          end

          return map
        end

        #Equivalent to build_attention_reference_profile_mapping with an :explicit value for the mode parameter.
        def build_explicit_attention_reference_profile_mapping(profile=nil)
          build_attention_reference_profile_mapping(:explicit,profile)
        end

        #Equivalent to build_attention_reference_profile_mapping with an :implicit value for the mode parameter.
        def build_implicit_attention_reference_profile_mapping(profile=nil)
          build_attention_reference_profile_mapping(:implicit,profile)
        end

        #Callback that initializes the model when the source object is saved.
        def setup_default_attention_profile
          if(AttentionProfile.find(:first,:conditions => ["attention_source_id=? AND attention_source_type=?",self.id, self.class.to_s])==nil)
            origin = AttentionProfile.new
            origin.name = "default"
            origin.attention_source = self
            origin.save!
          end
        end

        #Computes the total attention value for all the profiles and modes for this source
        def compute_total_implicit_attention_value
          total = 0
          attention_profiles.each do |profile|
            implicit_attention_destinies(profile).each do |dst|
              implicit_attention_points_for(dst,profile).each do |point|
                total += point.value
              end
            end
            implicit_attention_reference_destinies(profile).each do |ref|
              implicit_attention_points_for(ref,profile).each do |point|
                total += point.value
              end
            end
          end
          return total
        end

        #Computes the total explicit attention value for all the profiles for this source
        def compute_total_explicit_attention_value
          total = 0
          attention_profiles.each do |profile|
            explicit_attention_destinies(profile).each do |dst|
              explicit_attention_points_for(dst,profile).each do |point|
                total += point.value
              end
            end
            explicit_attention_reference_destinies(profile).each do |ref|
              explicit_attention_points_for(ref,profile).each do |point|
                total += point.value
              end
            end

          end
          return totla
        end

        #Computes the total implicit attention value for all the profiles for this source
        def compute_total_attention_value
          total = 0
          attention_profiles.each do |profile|
            explicit_attention_destinies(profile).each do |dst|
              explicit_attention_points_for(dst,profile).each do |point|
                total += point.value
              end
            end
            implicit_attention_destinies(profile).each do |dst|
              implicit_attention_points_for(dst,profile).each do |point|
                total += point.value
              end
            end
          end
          return total
        end

        #Obtains the APML representation ot the attention profile of this source.
        #Parameters hash:
        #[+calculator+] an object that implements the method compute_attention(concept,mode,value,total_attention_value) used
        #               to calculate the attention APML attribute for an AttentionDestiny or AttentionDestinyReference.
        #[+title+] value for the <Title> APML element.
        #[+generator+] value for the <Generator> APML element.
        #[+user_email+] value for the UserEmail APML element.
        #[+default_profile+] value for the 'default' attribute of the <Profile> APML element.
        def to_apml(options={ })

          builder = Bitnik::Apm::Builder.new(options[:calculator])

          if options[:title] != nil
            builder.title = options[:title]
          end

          if options[:generator] != nil
            builder.generator = options[:generator]
          end

          if options[:user_email] != nil
            builder.user_email = options[:user_email]
          end

          if options[:default_profile] != nil
            builder.default_profile = options[:default_profile]
          end

          if options[:from] != nil
            builder.from = options[:from]
          end

          builder.build_profile_for(self)
        end

        private

        def profile_or_default(attention_profile)
          if(attention_profile == nil)
            attention_profile = AttentionProfile.find(:first, :conditions => ["name=? AND attention_source_id=? AND attention_source_type=?","default",self.id,self.class.to_s])
          elsif attention_profile.class == String || attention_profile.class == Symbol
            attention_profile = AttentionProfile.find(:first, :conditions => ["name=? AND attention_source_id=? AND attention_source_type=?",attention_profile.to_s,self.id,self.class.to_s])
          else
            attention_profile
          end
        end

      end

      def check_destiny(attention_destiny)
        if(attention_destiny.class==AttentionDestinyReference || attention_destiny.class==AttentionDestiny)
          return attention_destiny
        else
          data = AttentionDestiny.find(:first,:conditions => ["attention_generator_id = ? AND attention_generator_type = ?",attention_destiny.id,attention_destiny.class.to_s])
          return data
        end
      end
    end

  end
end
