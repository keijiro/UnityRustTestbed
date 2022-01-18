use std::ffi::CStr;
use std::ffi::CString;
use std::os::raw::c_char;

//
// Bool args/return
//

#[no_mangle]
pub unsafe extern fn get_xor(a: bool, b: bool) -> bool {
    a ^ b
}

//
// Struct layout
//

#[repr(C)]
pub struct Data1 {
    d8: u8,
    d16: u16,
    d32: u32,
    d64: u64
}

#[repr(C)]
pub struct Data2 {
    d1: bool,
    d2: bool,
    d32: f32,
    d64: f64
}

#[no_mangle]
pub unsafe extern fn modify_struct_data1(src: *const Data1, dst: *mut Data1) {
    (*dst).d8  = (*src).d8  + 1;
    (*dst).d16 = (*src).d16 + 1;
    (*dst).d32 = (*src).d32 + 1;
    (*dst).d64 = (*src).d64 + 1;
}

#[no_mangle]
pub unsafe extern fn modify_inplace_struct_data1(data: *mut Data1) {
    (*data).d8  += 1;
    (*data).d16 += 1;
    (*data).d32 += 1;
    (*data).d64 += 1;
}

#[no_mangle]
pub unsafe extern fn modify_struct_data2(src: *const Data2, dst: *mut Data2) {
    (*dst).d1  = (*src).d1 ^ true;
    (*dst).d2  = (*src).d2 ^ true;
    (*dst).d32 = (*src).d32 + 1.0;
    (*dst).d64 = (*src).d64 + 1.0;
}

#[no_mangle]
pub unsafe extern fn modify_inplace_struct_data2(data: *mut Data2) {
    (*data).d1 ^= true;
    (*data).d2 ^= true;
    (*data).d32 += 1.0;
    (*data).d64 += 1.0;
}

//
// String operations
//

#[no_mangle]
pub unsafe extern fn count_substring(str_ptr: *const c_char, sub_ptr: *const c_char) -> u32 {
    let string = CStr::from_ptr(str_ptr).to_str().unwrap();
    let substring = CStr::from_ptr(sub_ptr).to_str().unwrap();
    string.matches(substring).count() as u32
}

#[no_mangle]
pub unsafe extern fn set_string(pointer: *mut c_char, size: i32) {
    let text = CString::new("Hi from Rust!").unwrap();
    let bytes = text.as_bytes_with_nul();
    let dest = std::slice::from_raw_parts_mut(pointer as *mut u8, size as usize);
    dest[..bytes.len()].copy_from_slice(bytes);
}

static mut FORMAT_FLOAT_BUFFER: Vec<u8> = vec![];

#[no_mangle]
pub unsafe extern fn format_float(value: f32) -> *const c_char {
    let text = CString::new(format!("{}", value)).unwrap();
    let bytes = text.as_bytes_with_nul();
    FORMAT_FLOAT_BUFFER.resize(bytes.len(), 0);
    FORMAT_FLOAT_BUFFER.copy_from_slice(bytes);
    FORMAT_FLOAT_BUFFER.as_ptr() as *const c_char
}
