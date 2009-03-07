module Ninject
  
  module Planning
    
    module Bindings
      
      class Binding
        
        def self.from_hash(service_type, config)
          binding = Binding.new service_type.to_clr_type
          binding.provider_callback = Ninject::Activation::Providers::StandardProvider.get_creation_callback(config[:to].to_clr_type) if config.respond_to? :to
          binding.target = BindingTarget.type;
        end
        
      end
      
    end
    
  end
  
end

class Initializer
  
  attr_accessor :bindings
  
  def initialize
    @bindings = []
  end
  
  def bind(service_type, config={})
    
  end
  
end

def to_configure_ninject(&b)
  raise ArgumentError.new("You need to provide a block for the initializer to work") if b.nil?
  initializer = Initializer.new
  b.call(initializer)
  initializer
end