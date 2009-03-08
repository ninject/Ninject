include Ninject::Activation
include Ninject::Activation::Providers
include Ninject::Dynamic::Activation::Providers
BindingTarget = Ninject::Planning::Bindings::BindingTarget

class Symbol
  
  def to_binding_target
    @@binding_target_mappings ||= {
      :type => BindingTarget.type,
      :self => BindingTarget.self,
      :provider => BindingTarget.provider,
      :method => BindingTarget.method,
      :constant => BindingTarget.constant      
    }
    @@binding_target_mappings[self]
  end
  
end

module System
  class Type
    def to_creation_callback
      StandardProvider.get_creation_callback self
    end
    
    def to_provider_callback
      System::Func.of(IContext, IProvider).new do |context|
        Ninject::ResolutionExtensions.get(context, self)
      end
    end
    
  end
  
  class Object
    def to_provider_callback      
      System.Func.of(IContext, IProvider).new { |context| self }
    end
    
    def to_constant_callback
      System::Func.of(IContext, IProvider).new do |context|
        ConstantProvider.new self
      end
    end
  end
end

class Proc
  def to_ruby_proc_callback
    System::Func.of(IContext, IProvider).new do |context|
      RubyProcProvider.new self
    end
  end
end

class Class
  def to_provider_callback
    self.to_clr_type.to_provider_callback
  end
  
  def to_creation_callback
    self.to_clr_type.to_creation_callback
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
                binding.provider_callback = config[:to].to_creation_callback
                config[:target] = :type
              when config[:provider]
                binding.provider_callback = config[:provider].to_provider_callback
                config[:target] = :provider
              when config[:method]
                binding.provider_callback = config[:method].to_ruby_proc_callback
                config[:target] = :method
              when config[:constant]
                binding.provider_callback = config[:constant].to_constant_callback
                config[:target] = :constant
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
  
  def clear_bindings
    @bindings=[]
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