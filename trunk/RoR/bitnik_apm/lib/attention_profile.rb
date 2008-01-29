class AttentionProfile < ActiveRecord::Base
  has_many :attention_points
  has_many :attention_reference_points
  belongs_to :attention_source, :polymorphic => true
end
