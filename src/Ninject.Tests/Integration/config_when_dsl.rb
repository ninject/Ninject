require File.dirname(__FILE__) + '/../Ninject.Tests.dll'
include Ninject::Tests::Fakes

to_configure_ninject do |ninject|
  ninject.bind IWeapon, :to => Shuriken do
    condition do |request|        
        request.target.nil? ? false : request.target.member.reflected_type == Samurai.to_clr_type
      end 
  end
  ninject.bind IWeapon, :to => Sword
  ninject.bind IWarrior, :to => Samurai
end