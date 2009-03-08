require File.dirname(__FILE__) + '/../Ninject.Tests.dll'
include Ninject::Tests::Fakes

to_configure_ninject do |ninject|
  ninject.bind IWeapon, :to => Sword, :meta => { :type => :melee }
  ninject.bind IWeapon, :to => Shuriken, :meta => { :type => "range" }
end