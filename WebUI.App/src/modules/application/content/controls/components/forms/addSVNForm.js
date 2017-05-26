import {connect} from 'react-redux';
import {componentDidMount} from 'react-lifecycle-decorators';
import {Field, reduxForm} from 'redux-form';

const validate = values => {
    const errors = {};
    if (!values.name) {
        errors.name = 'The Name field is required.'
    }
    
    if (!values.url) {
        errors.url = 'The Url field is required.'
    }
    
    if (!values.login) {
        errors.login = 'The Login field is required.'
    }
    
    if (!values.password) {
        errors.password = 'The Password field is required.'
    }
    
    return errors
};

const renderField = ({
    input, label, name, type, meta: {touched, error}
}) => (
    <div className="form-group">
        <label htmlFor={name}>{label}</label>
        <input {...input} type={type} placeholder={label} className="form-control"/>
        {
            touched &&
            error &&
            <div className="alert alert-danger" role="alert">
                <span className="field-validation-error">{error}</span>
            </div>
        }
    </div>
);

let AddSVNForm = ({handleSubmit}) => (
    <form onSubmit={ handleSubmit }>
        <div className="row">
            <div className="col-md-7">
                <Field
                    name="name"
                    type="text"
                    component={renderField}
                    label="Name"
                />
                <p>Friendly name</p>
                <Field
                    name="url"
                    type="text"
                    component={renderField}
                    label="Url"
                />
                <p>Enter the link to the root of your project repository</p>
                <Field
                    name="login"
                    type="text"
                    component={renderField}
                    label="Login"
                />
                <Field
                    name="password"
                    type="text"
                    component={renderField}
                    label="Password"
                />
                <button type="submit" className="btn btn-default btn-lg">Create</button>
            </div>
            <div className="col-sm-5">
                <img src="/Resources/Layout/Images/Help/SvnUrl.png" className="img-responsive"/>
            </div>
        </div>
    </form>
);

AddSVNForm = componentDidMount(
    (props) => {
    }
)(AddSVNForm);

const mapStateToProps = (state) => {
    return state;
};

AddSVNForm = connect(mapStateToProps)(AddSVNForm);

AddSVNForm = reduxForm({
   form: 'contact', validate
})(AddSVNForm);

export default AddSVNForm;

