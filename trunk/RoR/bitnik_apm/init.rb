require 'acts_as_attention_destiny_lib'
require 'acts_as_attention_source_lib'

ActiveRecord::Base.class_eval do
  include Bitnik::Acts::AttentionDestinyLib
  include Bitnik::Acts::AttentionSourceLib
end
