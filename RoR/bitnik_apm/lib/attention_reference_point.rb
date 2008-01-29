class AttentionReferencePoint < ActiveRecord::Base
  belongs_to :attention_destiny_reference
  belongs_to :attention_profile
end
