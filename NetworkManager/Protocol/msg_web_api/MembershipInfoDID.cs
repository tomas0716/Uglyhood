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

namespace sg.protocol.msg_web_api
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class MembershipInfoDID : TBase
  {
    private string _device_id;
    private int _i_BindType;

    public string Device_id
    {
      get
      {
        return _device_id;
      }
      set
      {
        __isset.device_id = true;
        this._device_id = value;
      }
    }

    public int I_BindType
    {
      get
      {
        return _i_BindType;
      }
      set
      {
        __isset.i_BindType = true;
        this._i_BindType = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool device_id;
      public bool i_BindType;
    }

    public MembershipInfoDID() {
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
            if (field.Type == TType.String) {
              Device_id = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              I_BindType = iprot.ReadI32();
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
      TStruct struc = new TStruct("MembershipInfoDID");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (Device_id != null && __isset.device_id) {
        field.Name = "device_id";
        field.Type = TType.String;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Device_id);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_BindType) {
        field.Name = "i_BindType";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_BindType);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MembershipInfoDID(");
      sb.Append("Device_id: ");
      sb.Append(Device_id);
      sb.Append(",I_BindType: ");
      sb.Append(I_BindType);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
