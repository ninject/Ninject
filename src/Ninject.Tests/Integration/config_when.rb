require File.dirname(__FILE__) + '/../Ninject.Tests.dll'
include Ninject::Tests::Fakes

to_configure_ninject do |ninject|
  ninject.bind IWeapon, :to => Shuriken, :when => { :injected_into => Samurai }
  ninject.bind IWeapon, :to => Sword
  ninject.bind IWarrior, :to => Samurai
end