const TextInput = ({
  name,
  label,
  value,
  onChange,
  errors = [],
  type = "text",
}) => {
  return (
    <div>
      <label class="form">{label}</label>
      <input
        type={type}
        name={name}
        className={`form ${errors.hasOwnProperty(name) ? "error" : ""}`}
        value={value}
        onChange={onChange}
      />
      {errors.length > 0 && <span class="form">{errors[0]}</span>}
    </div>
  );
};

export default TextInput;
