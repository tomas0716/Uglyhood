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
  public partial class MsgAnsGetAttendance : TBase
  {
    private sg.protocol.basic.Error _error;
    private Dictionary<int, sg.protocol.common.Attendance> _m_Attendance;
    private int _i_AttendanceCount;
    private int _i_IsTodayAttendance;

    public sg.protocol.basic.Error Error
    {
      get
      {
        return _error;
      }
      set
      {
        __isset.error = true;
        this._error = value;
      }
    }

    public Dictionary<int, sg.protocol.common.Attendance> M_Attendance
    {
      get
      {
        return _m_Attendance;
      }
      set
      {
        __isset.m_Attendance = true;
        this._m_Attendance = value;
      }
    }

    public int I_AttendanceCount
    {
      get
      {
        return _i_AttendanceCount;
      }
      set
      {
        __isset.i_AttendanceCount = true;
        this._i_AttendanceCount = value;
      }
    }

    public int I_IsTodayAttendance
    {
      get
      {
        return _i_IsTodayAttendance;
      }
      set
      {
        __isset.i_IsTodayAttendance = true;
        this._i_IsTodayAttendance = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool error;
      public bool m_Attendance;
      public bool i_AttendanceCount;
      public bool i_IsTodayAttendance;
    }

    public MsgAnsGetAttendance() {
      this._i_AttendanceCount = 0;
      this.__isset.i_AttendanceCount = true;
      this._i_IsTodayAttendance = 0;
      this.__isset.i_IsTodayAttendance = true;
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
            if (field.Type == TType.Struct) {
              Error = new sg.protocol.basic.Error();
              Error.Read(iprot);
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.Map) {
              {
                M_Attendance = new Dictionary<int, sg.protocol.common.Attendance>();
                TMap _map120 = iprot.ReadMapBegin();
                for( int _i121 = 0; _i121 < _map120.Count; ++_i121)
                {
                  int _key122;
                  sg.protocol.common.Attendance _val123;
                  _key122 = iprot.ReadI32();
                  _val123 = new sg.protocol.common.Attendance();
                  _val123.Read(iprot);
                  M_Attendance[_key122] = _val123;
                }
                iprot.ReadMapEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_AttendanceCount = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              I_IsTodayAttendance = iprot.ReadI32();
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
      TStruct struc = new TStruct("MsgAnsGetAttendance");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (Error != null && __isset.error) {
        field.Name = "error";
        field.Type = TType.Struct;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        Error.Write(oprot);
        oprot.WriteFieldEnd();
      }
      if (M_Attendance != null && __isset.m_Attendance) {
        field.Name = "m_Attendance";
        field.Type = TType.Map;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, M_Attendance.Count));
          foreach (int _iter124 in M_Attendance.Keys)
          {
            oprot.WriteI32(_iter124);
            M_Attendance[_iter124].Write(oprot);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (__isset.i_AttendanceCount) {
        field.Name = "i_AttendanceCount";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_AttendanceCount);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_IsTodayAttendance) {
        field.Name = "i_IsTodayAttendance";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_IsTodayAttendance);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgAnsGetAttendance(");
      sb.Append("Error: ");
      sb.Append(Error== null ? "<null>" : Error.ToString());
      sb.Append(",M_Attendance: ");
      sb.Append(M_Attendance);
      sb.Append(",I_AttendanceCount: ");
      sb.Append(I_AttendanceCount);
      sb.Append(",I_IsTodayAttendance: ");
      sb.Append(I_IsTodayAttendance);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
