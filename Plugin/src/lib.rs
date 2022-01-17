use std::ffi::CStr;
use std::ffi::CString;
use std::os::raw::c_char;

#[no_mangle]
pub unsafe extern fn count_substring(str_ptr: *const c_char, sub_ptr: *const c_char) -> u32 {
    let string = CStr::from_ptr(str_ptr).to_str().unwrap();
    let substring = CStr::from_ptr(sub_ptr).to_str().unwrap();
    string.matches(substring).count() as u32
}

#[no_mangle]
pub unsafe extern fn modify_string(pointer: *mut c_char, size: i32) {
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
