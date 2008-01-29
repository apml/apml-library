class AttentionDestiny < ActiveRecord::Base
  belongs_to :attention_generator, :polymorphic => true
end
