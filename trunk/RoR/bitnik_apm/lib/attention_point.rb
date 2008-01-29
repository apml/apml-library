class AttentionPoint < ActiveRecord::Base
  belongs_to :attention_destiny
  belongs_to :attention_profile
end
