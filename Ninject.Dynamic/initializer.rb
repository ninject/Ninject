include Ninject::Activation::Providers
BindingTarget = Ninject::Planning::Bindings::BindingTarget

class Symbol
  
  def to_binding_target
    case self
    when :type
      return BindingTarget.type
    when :self
      return BindingTarget.self
		when :provider
      return BindingTarget.provider
    when :method 
      return BindingTarget.method
    when :constant
      return BindingTarget.constant
    else
      return nil
    end
  end
  
end

module System
  class Type
    def to_creation_callback
      StandardProvider.get_creation_callback self
    end
    
    def to_provider
      
    end
  end
end

module Ninject
  
  module Planning
    
    module Bindings
      
      class Binding
        
        class << self
          
          def from_hash(config)
            binding = Binding.new config[:service].to_clr_type
            add_provider_callback binding, config 
            binding.target = (config[:target]||:self).to_sym.to_binding_target
            
            binding
          end
          
          private
            def add_provider_callback(binding, config)
              case 
              when config[:to] == :self || config[:to].nil?
                config[:target] = :self
                binding.provider_callback = binding.service.to_creation_callback
              when config[:to].is_a?(Class)
                binding.provider_callback = config[:to].to_clr_type.to_creation_callback
                config[:target] = :type
#              when config[:to] == :provider || config[:provider]
#                binding.provider_callback = config[:provider].to_clr_type
#                config[:target] = :provider
              #when 
              end
            end
        end
        
      end
      
    end
    
  end
  
end

class NinjectInitializer
  
  attr_accessor :bindings
  
  def initialize
    @bindings = []
  end
  
  def bind(service_type, config={})
    config[:service] ||= service_type
    @bindings << Ninject::Planning::Bindings::Binding.from_hash(config)
  end
  
end

def to_configure_ninject(&b)
  raise ArgumentError.new("You need to provide a block for the initializer to work") if b.nil?
  initializer = NinjectInitializer.new
  b.call(initializer)
  initializer.bindings
end

#to_configure_ninject do |ninject|
#  ninject.bind IServiceA, :to => ServiceA, :scope => :singleton
#end