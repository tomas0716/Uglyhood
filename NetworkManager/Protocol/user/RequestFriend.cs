﻿/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace sg.protocol.user
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class RequestFriend : TBase
  {
    private long _i_FriendUin;

    public long I_FriendUin
    {
      get
      {
        return _i_FriendUin;
      }
      set
      {
        __isset.i_FriendUin = true;
        this._i_FriendUin = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool i_FriendUin;
    }

    public RequestFriend() {
      this._i_FriendUin = 0;
      this.__isset.i_FriendUin = true;
    }

    public void Read (TProtocol iprot)
    {
      TField field;
      iprot.ReadStructBegin();
      while (true)
      {
        field = iprot.ReadFieldBegin();
        if (field.Type == TType.Stop) { 
          break;
        }
        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.I64) {
              I_FriendUin = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          default: 
            TProtocolUtil.Skip(iprot, field.Type);
            break;
        }
        iprot.ReadFieldEnd();
      }
      iprot.ReadStructEnd();
    }

    public void Write(TProtocol oprot) {
      TStruct struc = new TStruct("RequestFriend");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.i_FriendUin) {
        field.Name = "i_FriendUin";
        field.Type = TType.I64;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(I_FriendUin);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("RequestFriend(");
      sb.Append("I_FriendUin: ");
      sb.Append(I_FriendUin);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
