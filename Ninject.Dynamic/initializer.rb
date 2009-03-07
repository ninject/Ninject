include Ninject::Activation::Providers

module Ninject
  
  module Planning
    
    module Bindings
      
      class Binding
        
        def self.from_hash(service, config)
          binding = Binding.new config[:service].to_clr_type
          if config.respond_to? :to
            binding.provider_callback = StandardProvider.get_creation_callback(config[:to].to_clr_type)
            binding.target = BindingTarget.type
          end
          binding
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
    config[:service] ||= service_type
    @bindings << Binding.from_hash(config)
  end
  
end

def to_configure_ninject(&b)
  raise ArgumentError.new("You need to provide a block for the initializer to work") if b.nil?
  initializer = Initializer.new
  b.call(initializer)
  initializer
end