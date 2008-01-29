require 'active_record'

module Bitnik
  module Acts
    module AttentionDestinyLib

      def self.included(mod)
        mod.extend(ClassMethods)
      end

      module ClassMethods
        mattr_accessor :bitnik_apm_key_mapping
        mattr_accessor :bitnik_apm_from_value

        #Defines this model to be an attention destiny. An attention destiny is equivalent to a <Concept> in the APML specification.
        #This method will associate an attention_destiny method that gives access to the AttentionDestiny object associated to the model.
        #Params:
        #[+key_mapping+] a method that will be called to generate the 'key' attribute of the APML concept element.
        #                By default, the plugin will look for a key attribute in the model.
        #[+from_value+] the value for the from attribute of the APML from concept. If nothing is specified, the value will be set to 'default'.
        def acts_as_attention_destiny(options = {:key_mapping => nil, :from_value => 'default'})
          extend SingletonMethods
          include InstanceMethods

          has_one :attention_destiny, :as => :attention_generator, :dependent => :destroy

          if(options[:key_mapping] == nil)
            self.bitnik_apm_key_mapping = :key
          else
            self.bitnik_apm_key_mapping = options[:key_mapping]
          end

          if(options[:from_value]==nil)
            options[:from_value] = "default"
          else
            self.bitnik_apm_from_value = options[:from_value]
          end
          after_save :setup_attention_destiny

          extend Bitnik::Acts::AttentionDestinyLib::SingletonMethods
          include Bitnik::Acts::AttentionDestinyLib::InstanceMethods
        end
      end


      module SingletonMethods
      end

      module InstanceMethods

        #Callback that will be called after saving the model. It will associate an AttentionDestiny object to the model.
        def setup_attention_destiny
          if(AttentionDestiny.find(:first,:conditions => ["attention_generator_id=? AND attention_generator_type=?",self.id, self.class.to_s])==nil)
            origin = AttentionDestiny.new
            origin.is_explicit = false
            origin.is_implicit = true

            origin.key = self.send(self.class.bitnik_apm_key_mapping.to_sym)
            origin.from = self.class.bitnik_apm_from_value
            origin.attention_generator = self
            origin.save!
          end
        end

      end

    end
  end
end
