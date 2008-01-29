class BitnikApmFooObject < ActiveRecord::Base
  acts_as_attention_destiny :key_mapping => 'key'
end
